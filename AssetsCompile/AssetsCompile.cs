using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AssetsCompile
{

    public struct sFile
    {
        public string name;
        public string path;
        public string type;

        public int index;
        public int length;

        //
        public byte[] content;
    }

    public class AssetsCompile
    {
        List<sFile> _fileheaders;

        public List<sFile> getfileheaders()
        {
            return _fileheaders;
        }

        public void AddFile(string path_in_assets, string path)
        {
            sFile _fileheader = new sFile();
            FileInfo fileInfo = new FileInfo(path);

            _fileheader.name = fileInfo.Name;
            _fileheader.path = path_in_assets;
            _fileheader.type = fileInfo.Extension;
            _fileheader.content = File.ReadAllBytes(path);
            _fileheader.length = _fileheader.content.Length;

            _fileheaders.Add(_fileheader);
        }

        public void removeFile(string deletepath)
        {
            int deleteId = -1;
            for (int i = 0; i < _fileheaders.Count; i++)
            {
                Console.WriteLine(deletepath + "==" + _fileheaders[i].path);
                if (deletepath.ToUpper() == _fileheaders[i].path.ToUpper())
                {
                    deleteId = i;
                    Console.WriteLine("Find " + deletepath);
                    break;
                }
            }
            if (deleteId != -1)
            {
                _fileheaders.RemoveAt(deleteId);
                Console.WriteLine("delete : " + deleteId);
            }
        }

        public AssetsCompile()
        {
            _fileheaders = new List<sFile>();
        }

        public void Compile(string path, string password)
        {

            BinaryWriter content_bin = new BinaryWriter(new MemoryStream());

            for (int i = 0; i < _fileheaders.Count; i++)
            {
                sFile _fileheader = _fileheaders[i];
                _fileheader.index = (int)content_bin.BaseStream.Position;
                content_bin.Write(_fileheader.content);
            }

            content_bin.Flush();
            byte[] content_data = ((MemoryStream)content_bin.BaseStream).ToArray();
            content_bin.Close();

            BinaryWriter header_bin = new BinaryWriter(new MemoryStream());
            header_bin.Write(_fileheaders.Count);

            for (int i = 0; i < _fileheaders.Count; i++)
            {
                sFile _file = _fileheaders[i];

                header_bin.Write(_file.name);
                header_bin.Write(_file.path);
                header_bin.Write(_file.type);

                header_bin.Write(_file.index);
                header_bin.Write(_file.length);

            }

            header_bin.Flush();
            byte[] header_data = ((MemoryStream)header_bin.BaseStream).ToArray();
            header_bin.Close();


            BinaryWriter bin = new BinaryWriter(new MemoryStream());
            bin.Write(header_data.Length);
            bin.Write(header_data);
            bin.Write(content_data);

            bin.Flush();

            byte[] asset_data = ((MemoryStream)bin.BaseStream).ToArray();

            bin.Close();

            this.GenerateKeys();

            string path_file = Directory.GetParent(path).FullName;

            //32
            Console.WriteLine(KeyCrypto.Length);
            //16
            Console.WriteLine(IVCrypto.Length);

            byte[] key = new byte[KeyCrypto.Length + IVCrypto.Length];

            Array.Copy(KeyCrypto, key, KeyCrypto.Length);
            Array.Copy(IVCrypto, 0, key, KeyCrypto.Length, IVCrypto.Length);

             

            BinaryWriter Crypto_bin = new BinaryWriter(new MemoryStream());


            byte[] keyhash = EncryptKey(key, password);

            Crypto_bin.Write(keyhash.Length);
            Crypto_bin.Write(keyhash);

            for (int i = 0; i < keyhash.Length; i++)
                Console.Write(keyhash[i].ToString("x2"));
            Console.Write("\n");

            Crypto_bin.Write(EncryptStream(asset_data));
            

            Crypto_bin.Flush();


            File.WriteAllBytes(path, ((MemoryStream)Crypto_bin.BaseStream).ToArray());

            Crypto_bin.Close();
        }

      

        private byte[] KeyCrypto;
        private byte[] IVCrypto;

        public void GenerateKeys()
        {
            using (AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider())
            {
                aesCSP.GenerateKey();
                aesCSP.GenerateIV();

                KeyCrypto = aesCSP.Key;
                IVCrypto = aesCSP.IV;
            }
        }

        

        public byte[] EncryptKey(byte[] key, string password)
        {
            var keyGenerator = new Rfc2898DeriveBytes(password, 8);
            var rijndael = Rijndael.Create();

            rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
            rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);

            using (var fileStream = new BinaryWriter(new MemoryStream()))
            {

                fileStream.Write(keyGenerator.Salt);
                fileStream.Write(key.Length);
                

                var mscrypto = new MemoryStream();

                using (var cryptoStream = new CryptoStream(mscrypto, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(key);
                }

                byte[] keyhash = mscrypto.ToArray();

                fileStream.Write(keyhash.Length);
                fileStream.Write(keyhash);

                return ((MemoryStream) fileStream.BaseStream).ToArray();
            }
        }

        public byte[] DecryptKey(byte[] keyhash, string password, out bool isValide)
        {
            using (var fileStream = new BinaryReader( new MemoryStream(keyhash)))
            {
                // write random salt
                byte[] Salt = fileStream.ReadBytes(8);
                int length = fileStream.ReadInt32();
                int length_h = fileStream.ReadInt32();
                byte[] hash = fileStream.ReadBytes(length_h);

                var keyGenerator = new Rfc2898DeriveBytes(password, Salt);
                var rijndael = Rijndael.Create();

                rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
                rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);

                byte[] buffer = new byte[length];

                try
                {
                    var mscrypto = new MemoryStream(hash);
                    using (var cryptoStream = new CryptoStream(mscrypto, rijndael.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        cryptoStream.Read(buffer, 0, length);
                    }

                    isValide = true;
                }
                catch
                {
                    isValide = false;
                }

                return buffer;
            }

        }

        public byte[] EncryptStream(byte[] data)
        {
            using (AesCryptoServiceProvider Aes1 = new AesCryptoServiceProvider())
            {
                Aes1.Key = KeyCrypto;
                Aes1.IV = IVCrypto;

                ICryptoTransform encryptor = Aes1.CreateEncryptor();

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }

                    return ms.ToArray();
                }

            }
        }

        public byte[] DecryptStream(byte[] data, out bool isValide)
        {
            try
            {
                using (AesCryptoServiceProvider Aes1 = new AesCryptoServiceProvider())
                {
                    Aes1.Key = KeyCrypto;
                    Aes1.IV = IVCrypto;

                    ICryptoTransform encryptor = Aes1.CreateDecryptor();

                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Read))
                        {
                            byte[] buffer = new byte[data.Length];

                            cs.Read(buffer, 0, data.Length);

                            isValide = true;

                            return buffer;
                        }

                    }

                }
            }
            catch
            {
                isValide = false;
                return new byte[0];
            }
        }

        public void Read(byte[] data, string password, out bool isValide)
        {
            BinaryReader Crypto_bin = new BinaryReader(new MemoryStream(data));

            int length_key_hash = Crypto_bin.ReadInt32();
            
            byte[] keyIV = Crypto_bin.ReadBytes(length_key_hash);

            for (int i = 0; i < keyIV.Length; i++)
                Console.Write(keyIV[i].ToString("x2"));
            Console.Write("\n");

            byte[] datafile = Crypto_bin.ReadBytes(data.Length - length_key_hash - 4);
            Crypto_bin.Close();

            keyIV = DecryptKey(keyIV, password, out isValide);

            if (isValide)
            {

                byte[] key = new byte[32];
                Array.Copy(keyIV, key, 32);

                byte[] IV = new byte[16];
                Array.Copy(keyIV, 32, IV, 0, 16);

                KeyCrypto = key;
                IVCrypto = IV;

                datafile = DecryptStream(datafile, out isValide);
                if (isValide)
                {

                    BinaryReader bin = new BinaryReader(new MemoryStream(datafile));
                    int length_header = bin.ReadInt32();
                    int length_content = datafile.Length - length_header;

                    byte[] header = bin.ReadBytes(length_header);
                    byte[] content = bin.ReadBytes(length_content);

                    Console.WriteLine("=======");
                    Console.WriteLine(header.Length);
                    Console.WriteLine(content.Length);
                    Console.WriteLine("=======");

                    bin.Close();
                    BinaryReader bin_header = new BinaryReader(new MemoryStream(header));

                    int Count = bin_header.ReadInt32();

                    for (int i = 0; i < Count; i++)
                    {
                        sFile _file = new sFile(); ;

                        _file.name = bin_header.ReadString();

                        Console.WriteLine(_file.name);

                        _file.path = bin_header.ReadString();
                        _file.type = bin_header.ReadString();

                        _file.index = bin_header.ReadInt32();
                        _file.length = bin_header.ReadInt32();


                        _fileheaders.Add(_file);
                    }

                    bin_header.Close();

                    BinaryReader bin_content = new BinaryReader(new MemoryStream(content));
                    for (int i = 0; i < Count; i++)
                    {
                        sFile _fileheader = _fileheaders[i];
                        _fileheader.content = bin_content.ReadBytes(_fileheader.length);

                        Console.WriteLine(_fileheader.content.Length + " == " + _fileheader.length);

                        byte[] test = new byte[128];
                        Array.Copy(_fileheader.content, test, 128);

                        Console.WriteLine(Encoding.UTF8.GetString(test));
                        Console.WriteLine("==");

                        _fileheaders[i] = _fileheader;
                    }
                }
                else
                {
                    Console.WriteLine("Not valide !");
                }
            }
            else
            {
                Console.WriteLine("Not valide !");
            }
         }
    }
}