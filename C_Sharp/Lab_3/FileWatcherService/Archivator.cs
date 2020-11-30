﻿using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace FileWatcherService
{
    class Archivator
    {
        private static string encryptedFileName;

        private static string decryptedFileName;

        public static string errorFile;

        public static void Archive(string fileName, string targetDir, bool encrypt)
        {
            encryptedFileName = Encryptor.GetTargetEncryptedFilePath(fileName, targetDir);
            try
            {
                using (var memory = new MemoryStream())
                {
                    using (var zip = new ZipArchive(memory, ZipArchiveMode.Create, true))
                    {
                        var memoryFile = zip.CreateEntry(Path.GetFileName(encryptedFileName));
                        FileStream sourceStream = default;

                        while (true)
                        {
                            try
                            {
                                sourceStream = new FileStream(fileName, FileMode.Open);
                            }
                            catch (IOException)
                            {
                                continue;
                            }
                            break;
                        }

                        if (encrypt == true)
                        {
                            using (Stream targetEncryptedStream = memoryFile.Open())
                            {
                                Encryptor.Encrypt(sourceStream, targetEncryptedStream);
                            }
                        }

                        sourceStream.Close();
                        sourceStream.Dispose();
                    }

                    using (var encryptedFileStream = new FileStream(Path.Combine(targetDir, Path.GetFileNameWithoutExtension(fileName) + ".zip"), FileMode.Create))
                    {
                        memory.Seek(0, SeekOrigin.Begin);
                        memory.CopyTo(encryptedFileStream);
                    }
                }
            }
            catch (Exception e)
            {
                errorFile = String.Concat(Directory.GetCurrentDirectory(), "Errors.txt");

                using (var errorStream = new StreamWriter(new FileStream(errorFile, FileMode.OpenOrCreate)))
                {
                    errorStream.Write(e.Message + "\n\n" + e.StackTrace);
                }
            }
        }

        public static void Dearchive(string fileName, string targetDir)
        {
            try
            {
                using (var zip = ZipFile.OpenRead(fileName))
                {
                    var file = zip.Entries[0];

                    decryptedFileName = Encryptor.GetTargetDecryptedFilePath(file.Name, targetDir);

                    using (var targetStream = new FileStream(decryptedFileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (Stream targetDecryptedStream = file.Open())
                        {
                            Encryptor.Decrypt(targetDecryptedStream, targetStream);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorFile = String.Concat(Directory.GetCurrentDirectory(), "Errors.txt");

                using (var errorStream = new StreamWriter(new FileStream(errorFile, FileMode.OpenOrCreate)))
                {
                    errorStream.Write(e.Message + "\n\n" + e.StackTrace);
                }
            }

            Thread.Sleep(100);
        }
    }
}
