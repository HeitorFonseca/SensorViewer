using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Serialization.Formatters.Soap;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Windows.Controls;

namespace SensorsViewer
{
    /// <summary>
    /// StatePersistence of GUI data.
    /// </summary>
    public class StatePersistence
    {
        /// <summary>
        /// Singleton for static operations
        /// </summary>
        private static StatePersistence singleton;

        /// <summary>
        /// Private global form
        /// </summary>
        private Window window;

        /// <summary>
        /// Private form state filename
        /// </summary>
        private string formStateFileName = string.Empty;

        /// <summary>
        /// Private form values
        /// </summary>
        private NameValueCollection formValues = null;

        /// <summary>
        /// Boolean for save control
        /// </summary>
        private bool alreadySaved = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Denso.Pcs.StatePersistence" /> class 
        /// </summary>
        /// <param name="window">Current form</param>
        public StatePersistence(Window window)
        {
            if (window != null)
            {
                this.window = window;
                this.formStateFileName = Path.Combine(
                                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                        this.window.Name + ".xml");
                this.window.Loaded += new RoutedEventHandler(this.Window_Loaded);
                this.window.Closing += new CancelEventHandler(this.Window_Closing);
            }
        }

        /// <summary>
        /// Retrieve form state.
        /// </summary>
        /// <param name="loadedFilePath"> The path of file to be loaded.</param>
        private void RetrieveFormState(string loadedFilePath)
        {
            var filePath = loadedFilePath;

            // IF the filePath is empty, load the default xml file with the windows form name.
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = this.formStateFileName;
            }

            // Use the xml if the file Exist.
            if (File.Exists(filePath))
            {
                object o = null;
                using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    SoapFormatter formatter = new SoapFormatter();
                    o = formatter.Deserialize(stream);
                    this.formValues = (NameValueCollection)o;
                }
            }
            else
            {
                this.formValues = new NameValueCollection();
            }
        }

        /// <summary>
        /// Load all controls from forms
        /// </summary>
        /// <param name="loadedFilePath">The path of file to be loaded.</param>
        private void LoadControlValues(string loadedFilePath)
        {
            // Retrieve the state of the form using the xml file.
            this.RetrieveFormState(loadedFilePath);

            // Load textbox contents
            foreach (TextBox tb in FindVisualChildren<TextBox>(this.window))
            {
                if (tb.Name != "tbLogOutput")
                {
                    tb.Text = this.formValues.Get(tb.Name);
                }
            }

            // Load checkbox contents
            foreach (CheckBox cb in FindVisualChildren<CheckBox>(this.window))
            {
                var checkStatus = this.formValues.Get(cb.Content.ToString());

                if (checkStatus != null)
                {
                    cb.IsChecked =
                        checkStatus.Equals("true", StringComparison.CurrentCultureIgnoreCase) ?
                        true :
                        false;
                }
            }
        }

        /// <summary>
        /// Save form state
        /// </summary>
        /// <param name="fileToSave"> The path of file.</param>
        private void SaveFormState(string fileToSave)
        {
            Stream s = null;
            string file = fileToSave;

            try
            {
                s = new FileStream(file, FileMode.Create, FileAccess.Write);
                SoapFormatter formatter = new SoapFormatter();
                formatter.Serialize(s, this.formValues);

                using (StreamWriter w = new StreamWriter(s))
                {
                    s = null;
                    w.Flush();
                }
            }
            finally
            {
                if (s != null)
                {
                    s.Dispose();
                }
            }
        }

        /// <summary>
        /// Save group box controls
        /// </summary>
        /// <param name="fileToSave">The path of file to be saved.</param>
        private void SaveControlValues(string fileToSave)
        {
            if (this.alreadySaved == false)
            {
                // Save textbox contents
                foreach (TextBox tb in FindVisualChildren<TextBox>(this.window))
                {
                    if (tb.Name != "tbLogOutput")
                    {
                        this.formValues.Set(tb.Name, tb.Text);
                    }
                }

                // Save checkbox contents
                foreach (CheckBox cb in FindVisualChildren<CheckBox>(this.window))
                {
                    this.formValues.Set(cb.Content.ToString(), cb.IsChecked.ToString());
                }

                // Save the form in the xml file
                this.SaveFormState(fileToSave);
            }
        }

        /// <summary>
        /// Register window to have its forms persisted
        /// </summary>
        /// <param name="window">Window to be persisted</param>
        public static void RegisterWindow(Window window)
        {
            singleton = new StatePersistence(window);
        }

        /// <summary>
        /// Event when window is loaded
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="System.EventArgs" /> contains the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadControlValues(this.formStateFileName);
        }

        /// <summary>
        /// Event when window is closing
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="System.EventArgs" /> contains the event data.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.SaveControlValues(this.formStateFileName);
            this.alreadySaved = true;
        }

        /// <summary>
        /// Finds visual children of a visual object by type
        /// </summary>
        /// <typeparam name="T">Visual type to be searched</typeparam>
        /// <param name="depObj">Root visual object</param>
        /// <returns>The list of children of determined type</returns>
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
