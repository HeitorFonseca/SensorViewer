// <copyright file="OptionVm.cs" company="GM">
//     gm.com. All rights reserved.
// </copyright>

namespace SensorsViewer.Home
{
    using System;

    /// <summary>
    /// Option for left menu bar class
    /// </summary>
    public class OptionVm
    {
        /// <summary>
        /// Gets os sets idCount
        /// </summary>
        private static int idCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionVm"/> class
        /// </summary>
        public OptionVm()
        {
            this.Id = idCount++;
            this.Tags = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionVm"/> class
        /// </summary>
        /// <param name="title">Option Title</param>
        public OptionVm(string title)
        {
            this.Id = idCount++;
            this.Title = title;
            this.Tags = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionVm"/> class
        /// </summary>
        /// <param name="title">Option Title</param>
        /// <param name="content">Option content</param>
        /// <param name="tags">Option tags</param>
        public OptionVm(string title, Type content, string tags = "")
        {
            this.Id = idCount++;
            this.Title = title;
            this.Content = content;
            this.Tags = tags;
        }

        /// <summary>
        /// Gets Id
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Content
        /// </summary>
        public Type Content { get; set; }

        /// <summary>
        /// Gets or sets Tags
        /// </summary>
        public string Tags { get; set; }
    }
}
