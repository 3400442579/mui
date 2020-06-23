using An.Editor.Util;
using An.Editor.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace An.Editor.Models
{
    public class Project : ReactiveObject
    {
        public static Project Instance = new Project();

        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    this.RaisePropertyChanged(nameof(Width));
                }
            }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set
            {
                if (height != value)
                {
                    height = value;
                    this.RaisePropertyChanged(nameof(Height));
                }
            }
        }

        public List<Frame> Frames { get; } = new List<Frame>();

        public string Name { get; set; }

        public bool IsNew { get; set; }

        public double Dpi { get; set; } = 96;

        /// <summary>
        /// The base bit depth of the project.
        /// 32 is RGBA
        /// 24 is RGB
        /// </summary>
        public double BitDepth { get; set; } = 32;

        public DateTime Createtime { get; set; }

        public string RelativePath { get; set; }

        /// <summary>
        /// The full path of project based on current settings.
        /// </summary>
        public string FullPath => Path.Combine(Config.All.TemporaryFolderResolved, "AnEditor", "Recording", RelativePath);

        /// <summary>
        /// Full path to the serialized project file. 
        /// </summary>
        public string ProjectPath => Path.Combine(FullPath, "Project.json");

        /// <summary>
        /// The full path to the action stack files (undo, redo).
        /// </summary>
        public string ActionStackPath => Path.Combine(FullPath, "ActionStack");

        /// <summary>
        /// The full path to the undo folder.
        /// </summary>
        public string UndoStackPath => Path.Combine(ActionStackPath, "Undo");

        /// <summary>
        /// The full path to the redo folder.
        /// </summary>
        public string RedoStackPath => Path.Combine(ActionStackPath, "Redo");

        /// <summary>
        /// The full path to the blob file, used by the recorder to write all frames pixels as a byte array, separated by a delimiter.
        /// </summary>
        public string CachePath => Path.Combine(Config.All.TemporaryFolderResolved, "AnEditor", "Recording", RelativePath, "Frames.cache");


        /// <summary>
        /// Check if there's any frame on this project.
        /// </summary>
        public bool Any => Frames != null && Frames.Any();

        /// <summary>
        /// The latest index of the current list of frames, or -1.
        /// </summary>
        public int LatestIndex => Frames?.Count - 1 ?? -1;

        #region Methods

        public Project CreateProjectFolder()
        {
            //IsNew = true;
            RelativePath = DateTime.Now.ToString("yyMMddHHmmss") + Path.DirectorySeparatorChar;


            Directory.CreateDirectory(FullPath);

            #region Create ActionStack folders

            if (!Directory.Exists(ActionStackPath))
                Directory.CreateDirectory(ActionStackPath);

            if (!Directory.Exists(UndoStackPath))
                Directory.CreateDirectory(UndoStackPath);

            if (!Directory.Exists(RedoStackPath))
                Directory.CreateDirectory(RedoStackPath);

            #endregion

            CreateMutex();

            return this;
        }

        public void Persist()
        {
            try
            {
                File.WriteAllText(ProjectPath, JsonSerializer.Serialize(this,
                    new JsonSerializerOptions { IgnoreNullValues = true, AllowTrailingCommas = false }));
            }
            catch (Exception ex)
            {
                // LogWriter.Log(ex, "Persisting the current project info.");
            }
        }

        public void Clear()
        {
            Frames?.Clear();

            MutexList.Remove(RelativePath);
        }

        //public string FilenameOf(int index)
        //{
        //    return Any && LatestIndex >= index ? Path.Combine(FullPath, Frames[index].Name) : "";
        //}

        /// <summary>
        /// Gets the index that is in range of the current list of frames.
        /// </summary>
        /// <param name="index">The index to compare.</param>
        /// <returns>A valid index.</returns>
        public int ValidIndex(int index)
        {
            if (index == -1)
                index = 0;

            return LatestIndex >= index ? index : LatestIndex;
        }

        public void CreateMutex()
        {
            //TODO: Validate the possibility of openning this project.
            //I need to make sure that i'll release the mutexes.

            MutexList.Add(RelativePath);
        }

        public void ReleaseMutex()
        {
            MutexList.Remove(RelativePath);
        }

        #endregion
    }
}
