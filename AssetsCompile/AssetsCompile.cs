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

            for(int i = 0; i < key.Length ; i ++)
                Console.Write(key[i].ToString("x2"));
            Console.Write("\n");

            BinaryWriter Crypto_bin = new BinaryWriter(new MemoryStream());


            //byte[] keyhash = EncryptKey(key, password);
            Crypto_bin.Write(key.Length);
            Crypto_bin.Write(key);
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

        private const int SaltSize = 8;

        public byte[] EncryptKey(byte[] key, string password)
        {
            var keyGenerator = new Rfc2898DeriveBytes(password, SaltSize);
            //keyGenerator.Salt

            var rijndael = Rijndael.Create();
 
            rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
            rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);

            using (MemoryStream ms = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(ms, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(key, 0, key.Length); 
                }

                return ms.ToArray();
            }
        }

        public byte[] DecryptKey(byte[] keyhash, string password)
        {
            var keyGenerator = new Rfc2898DeriveBytes(password, SaltSize);
            //keyGenerator.Salt

            var rijndael = Rijndael.Create();

            rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
            rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);

            using (MemoryStream ms = new MemoryStream(keyhash))
            {
                using (var cryptoStream = new CryptoStream(ms, rijndael.CreateEncryptor(), CryptoStreamMode.Read))
                {
                    byte[] buffer = new byte[keyhash.Length];
                    cryptoStream.Read(buffer);

                    return buffer;
                }
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

        public byte[] DecryptStream(byte[] data)
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

                        return buffer;
                    }
                     
                }

            }
        }

        public void Read(byte[] data, string password)
        {
            BinaryReader Crypto_bin = new BinaryReader(new MemoryStream(data));

            int length = Crypto_bin.ReadInt32();
            byte[] keyIV = Crypto_bin.ReadBytes(length);
            byte[] datafile = Crypto_bin.ReadBytes(data.Length - length - 4);
            Crypto_bin.Close();

            //keyIV = DecryptKey(keyIV, password);

            for (int i = 0; i < keyIV.Length; i++)
                Console.Write(keyIV[i].ToString("x2"));
            Console.Write("\n");

            byte[] key = new byte[32];
            Array.Copy(keyIV, key, 32);

            byte[] IV = new byte[16];
            Array.Copy(keyIV, 32, IV, 0, 16);

            KeyCrypto = key;
            IVCrypto = IV;

            datafile = DecryptStream(datafile);
            BinaryReader bin = new BinaryReader(new MemoryStream(datafile));
            int length_header =  bin.ReadInt32();
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

            BinaryReader bin_content  = new BinaryReader(new MemoryStream(content));
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
    }
}