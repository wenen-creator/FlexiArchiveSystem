using System;
using System.Threading.Tasks;
using System.IO;

namespace FlexiArchiveSystem.ArchiveOperation.IO
{
    public class IOHelper
    {
        public static bool FileIsInUse(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                return false;
            }

            try
            {
                using (FileStream fileStream =
                       new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None));
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }
        
        public static async Task<bool> FileIsInUse(string filePath, int timeout , int intervalCheckTime, bool isImmeQuery = false)
        {
            if (File.Exists(filePath) == false)
            {
                return false;
            }

            if (intervalCheckTime < 20 || intervalCheckTime >= timeout)
            {
                throw new Exception("间隔检测时间无效");
            }
            
            float time = 0;
            if (isImmeQuery == false)
            {
                time = intervalCheckTime;
                await Task.Delay(intervalCheckTime);
            }
            
            while (time < timeout)
            {
                try
                {
                    using (FileStream fileStream =
                           new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None));
                    
                    //No Use
                    return false;
                }
                catch (IOException)
                {
                    time += intervalCheckTime;
                    if (time < timeout)
                    {
                        await Task.Delay(intervalCheckTime);
                    }
                }
            }
            return true;
        }
        
        public static async void FileIsInUse(string filePath, int timeout , int intervalCheckTime,Action noUseAction, bool isImmeQuery = false)
        {
            if (intervalCheckTime < 20 || intervalCheckTime >= timeout)
            {
                throw new Exception("间隔检测时间无效");
            }
            
            float time = 0;
            if (isImmeQuery == false)
            {
                time = intervalCheckTime;
                await Task.Delay(intervalCheckTime);
            }
            
            while (time < timeout)
            {
                try
                {
                    using (FileStream fileStream =
                           new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None));
                    
                    //No Use
                    noUseAction?.Invoke();
                }
                catch (IOException)
                {
                    time += intervalCheckTime;
                    if (time < timeout)
                    {
                        await Task.Delay(intervalCheckTime);
                    }
                }
            }
        }
    }
}