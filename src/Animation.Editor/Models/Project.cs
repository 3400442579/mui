using Animation.Editor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;

namespace Animation.Editor.Models
{
    public enum ProjectByType
    {
        Unknown = 0,
        ScreenRecorder = 1,
        WebcamRecorder = 2,
        BoardRecorder = 3,
        Editor = 4,
    }

    public class Project
    {
        private static Dictionary<string, Mutex> All { get; set; } = new Dictionary<string, Mutex>();


        //Full path to the serialized project file. 
        private string savePath;

        public int Width { get;  set; }
        public int Height { get; set; }

        public string ProjectName { get;private set; }
        public DateTime CreateTime { get; private set; } 

        public ProjectByType CreatedBy { get; private set; }

        public List<Frame> Frames { get; set; }


        /// <summary>
        /// The full path of project based on current settings.
        /// </summary>
        public string FullPath { get; private set; }

      
        /// <summary>
        /// 撤消文件夹的完整路径.
        /// </summary>
        public string UndoStackPath => Path.Combine(FullPath,"ur", "U");

        /// <summary>
        /// 重做文件夹的完整路径
        /// </summary>
        public string RedoStackPath => Path.Combine(FullPath, "ur", "R");

        /// <summary>
        /// Check if there's any frame on this project.
        /// </summary>
        public bool Any => Frames != null && Frames.Any();

        /// <summary>
        /// The latest index of the current list of frames, or -1.
        /// </summary>
        public int LatestIndex => Frames?.Count - 1 ?? -1;


        public static Project Create(string folder, ProjectByType? creator=null) {
            string name = DateTime.Now.ToString("yyMMddHHmmss");
            Project project = new Project
            {
                ProjectName = name,
                CreatedBy = creator ?? ProjectByType.Unknown,
                CreateTime = DateTime.Now,
                FullPath = Path.Combine(folder, "Project", name)
            };
            project.savePath = Path.Combine(project.FullPath, "info.json");

            //
            Directory.CreateDirectory(project.FullPath);
            if (!Directory.Exists(project.UndoStackPath))
                Directory.CreateDirectory(project.UndoStackPath);
            if (!Directory.Exists(project.RedoStackPath))
                Directory.CreateDirectory(project.RedoStackPath);

            project.CreateMutex();

            return project;
        }

        public void CreateMutex()
        {
            //TODO: Validate the possibility of openning this project.
            //I need to make sure that i'll release the mutexes.

            Add(ProjectName);
        }
        public void Persist()
        {
            try
            {
                Json.SerializeToFile(this, savePath);
            }
            catch (Exception ex)
            {
               // LogWriter.Log(ex, "Persisting the current project info.");
            }
        }

        public void Clear()
        {
            Frames?.Clear();

            Remove(ProjectName);
        }

        public void ReleaseMutex()
        {
            Remove(ProjectName);
        }

        internal static void RemoveAll()
        {
            foreach (var mutex in All)
                mutex.Value.Dispose();

            All.Clear();
            GC.Collect();
        }
        internal static void Add(string key)
        {
            if (All.ContainsKey(key))
                Remove(key);

            var mutex = new Mutex(false, @"Global\AnimationEditor" + key.Replace("\\",""), out bool created);
            mutex.GetAccessControl().AddAccessRule(new MutexAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, MutexRights.FullControl, AccessControlType.Allow));
           
            All.Add(key, mutex);
        }

        internal static bool Exists(string key)
        {
            return All.Any(f => f.Key == key);
        }

        internal static void Remove(string key)
        {
            var current = All.FirstOrDefault(f => f.Key == key).Value;

            if (current == null)
                return;

            //current.ReleaseMutex();
            current.Dispose();

            All.Remove(key);

            GC.Collect();
        }
    }
}
