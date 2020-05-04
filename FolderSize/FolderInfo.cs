using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.InteropServices;

namespace InSoft.IO
{
    public class FolderInfo
    {
        public List<FolderInfo> Folders { get; private set; }
        public List<FileInfo> Files { get; private set; }
        public string Path { get; private set; }
        public long Bytes { get; private set; }

        public double KiloBytes
        {
            get
            {
                return Bytes / 1024.0;
            }
        }

        public double MegaBytes
        {
            get
            {
                return Bytes / 1024.0 / 1024.0;
            }
        }

        public double GigaBytes
        {
            get
            {
                return Bytes / 1024.0 / 1024.0 / 1024.0;
            }
        }

        private FolderInfo()
        {
            this.Folders = new List<FolderInfo>();
            this.Files = new List<FileInfo>();
        }

        public static FolderInfo GetInfo(string path)
        {
            int clusterSize = GetClusterSize(System.IO.Path.GetPathRoot(path));

            return GetInfo(path, clusterSize);
        }

        private static FolderInfo GetInfo(string path, int clusterSize)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            DirectoryInfo[] innerDirectories = null;
            FileInfo[] innerFiles = null;

            try
            {
                innerDirectories = directoryInfo.GetDirectories();
                innerFiles = directoryInfo.GetFiles();
            }
            catch (Exception)
            {

            }

            FolderInfo folderInfo = new FolderInfo();

            folderInfo.Path = path;

            if (innerFiles != null)
            {
                folderInfo.Files.AddRange(innerFiles);
            }

            if (innerDirectories != null)
            {
                foreach (var item in innerDirectories)
                {
                    folderInfo.Folders.Add(GetInfo(item.FullName, clusterSize));
                }
            }

            long sizeOfFolder = 0;

            foreach (var item in folderInfo.Files)
            {
                long sizeOnDisk = 0;

                sizeOnDisk = (long)Math.Ceiling((double)item.Length / clusterSize) * clusterSize;

                sizeOfFolder += sizeOnDisk;
            }

            foreach (var item in folderInfo.Folders)
            {
                sizeOfFolder += item.Bytes;
            }

            folderInfo.Bytes = sizeOfFolder;

            return folderInfo;
        }

        private static int GetClusterSize(string rootDrive)
        {
            uint dummy, sectorsPerCluster, bytesPerSector;
            GetDiskFreeSpaceW(rootDrive, out sectorsPerCluster, out bytesPerSector, out dummy, out dummy);

            return (int)(sectorsPerCluster * bytesPerSector);
        }

        //public static long GetFileSizeOnDisk(string file)
        //{
        //    FileInfo info = new FileInfo(file);
        //    uint dummy, sectorsPerCluster, bytesPerSector;
        //    int result = GetDiskFreeSpaceW(info.Directory.Root.FullName, out sectorsPerCluster, out bytesPerSector, out dummy, out dummy);
        //    if (result == 0) throw new Win32Exception();
        //    uint clusterSize = sectorsPerCluster * bytesPerSector;
        //    uint hosize;
        //    uint losize = GetCompressedFileSizeW(file, out hosize);
        //    long size;
        //    size = (long)hosize << 32 | losize;
        //    return ((size + clusterSize - 1) / clusterSize) * clusterSize;
        //}

        //[DllImport("kernel32.dll")]
        //static extern uint GetCompressedFileSizeW([In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
        //   [Out, MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);

        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
        static extern int GetDiskFreeSpaceW([In, MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName,
           out uint lpSectorsPerCluster, out uint lpBytesPerSector, out uint lpNumberOfFreeClusters,
           out uint lpTotalNumberOfClusters);
    }
}