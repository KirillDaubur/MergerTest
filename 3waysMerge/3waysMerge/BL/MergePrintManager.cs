using _3waysMerge.Models;
using _3waysMerge.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace _3waysMerge.BL
{
    public class MergePrintManager
    {
        public void PrintThreeWayMergeResult(string resultPath, List<ThreeWayMergedDocumentLine> lines)
        {
            string resultText = "";

            for (int i = 0; i < lines.Count;)
            {
                switch (lines[i].State)
                {
                    case ThreeWayMergeDocumentLineState.Conflict:
                        resultText += $"================== Conflict: Parent {lines[i].Parent.ToString()} ==================\r\n";
                        PatentType p = lines[i].Parent;

                        while(i < lines.Count && lines[i].Parent == p && lines[i].State == ThreeWayMergeDocumentLineState.Conflict)
                        {
                            resultText += lines[i].Text + "\r\n";
                            i++;
                        }

                        resultText += $"============================================================\r\n";

                        break;
                    case ThreeWayMergeDocumentLineState.Removed:
                        i++;
                        break;
                    default:
                        resultText += lines[i].Text + "\r\n";
                        i++;
                        break;
                }
            }

            try
            {
                if (!File.Exists(resultPath))
                {
                    FileStream fs = File.Create(resultPath);
                    fs.Close();
                }
                using (StreamWriter sw = new StreamWriter(resultPath, false, Encoding.Default))
                {
                    sw.Write(resultText.Remove(resultText.Length - 2));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
