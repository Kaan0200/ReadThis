using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace readthis_doc_maker
{
    class Program
    {
        static void Main(string[] args)
        {
            // identify run options
            Dictionary<RunOptions, bool> modePairs = new Dictionary<RunOptions, bool>();
            modePairs.Add(RunOptions.Silent, false);
            modePairs.Add(RunOptions.Interactive, false);
            modePairs.Add(RunOptions.Opinionated, false);

            if (args.Length != 0) {
                foreach (string arg in args) {
                    switch (arg) {
                        case ("-s"): 
                            modePairs[RunOptions.Silent] = true;
                        break;
                        case ("-i"):
                            modePairs[RunOptions.Interactive] = true;
                        break;
                        case ("-o"):
                            modePairs[RunOptions.Opinionated] = true;
                        break;
                    }
                }
            } else {
                // default behavior
                modePairs[RunOptions.Interactive] = true;
                modePairs[RunOptions.Opinionated] = true;
            }

            // initilize
            StreamWriter streamOut = new StreamWriter("READTHIS.MD");
            DirectoryInfo rootInfo = new DirectoryInfo(".");
            DirectoryInfo[] dirInfos = rootInfo.GetDirectories();
            FileInfo[] fileInfos = rootInfo.GetFiles();

            // write header information
            if (modePairs[RunOptions.Interactive]) {
                streamOut.WriteLine(FormatHeader(DoPrompt("Describe what the current folder is for..."), dirInfos.Length, fileInfos.Length));
            } else {
                streamOut.WriteLine(FormatHeader(string.Empty, dirInfos.Length, fileInfos.Length));
            }
            streamOut.WriteLine(Break());
            
            // loop through directories
            foreach (DirectoryInfo dir in dirInfos) {
                streamOut.WriteLine("./" + dir.Name);
                streamOut.WriteLine(FormatAttributes(dir.Attributes));
                streamOut.WriteLine();
            }
            // loop through files
            foreach (FileInfo file in fileInfos) {
                streamOut.WriteLine("  " + file.Name);
                streamOut.WriteLine(FormatAttributes(file.Attributes));
                streamOut.WriteLine();
            }

            // clean up
            Console.Clear();
            streamOut.Close();
            
        }

        static string DoPrompt(string prompt) {
            
                Console.Clear();
                Console.WriteLine(prompt);
                return Console.ReadLine();
        }

        static string Break() {
            return "---------------------------------------\n";
        }

        static string FormatHeader(string description, int dirCount, int fileCount) {
            FileInfo currFile = new FileInfo(Environment.CurrentDirectory);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("# {0}\n", currFile.Name);
            //sb.AppendFormat("## @ {1}\n", currFile);
            if (description != string.Empty) {
                sb.AppendFormat("_{0}_\n", description);
            } else {
                sb.Append("\n");
            }
            
            sb.Append(Break());
            sb.AppendFormat("By: {0}, created at: {1} \n", Environment.UserName, DateTime.Now);
            sb.AppendFormat("{0} dirs | {1} fldrs | {2} all \n", dirCount, fileCount, dirCount + fileCount);
            return sb.ToString();
        }

        static string FormatAttributes(FileAttributes attributes) {
            if((attributes & FileAttributes.System) == FileAttributes.System) {
                return "> [SYSTEM]";
            } else {
                return "";
            }
            
        }
    }

    enum RunOptions {
        Interactive, // ask a question for each file
        Opinionated, // write suggestive information, like if file hasn't been touched in years
        Silent, // no prompts
    }
}
