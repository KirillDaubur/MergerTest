using _3waysMerge.BL.Extensions;
using _3waysMerge.Models;
using _3waysMerge.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace _3waysMerge.BL
{
    public class MergeManager
    {

        public async Task<List<ThreeWayMergedDocumentLine>> GetThreeWayMergeResultAsync(string parentPath, string leftChildPath, string rightChildPath)
        {
            List<ThreeWayMergedDocumentLine> result = new List<ThreeWayMergedDocumentLine>();

            List<TwoWayMergedDocumentLine> leftResult;
            List<TwoWayMergedDocumentLine> rightResult;

            Task<List<TwoWayMergedDocumentLine>> t1 = GetTwoWayMergeResultAsync(parentPath, leftChildPath);
            Task<List<TwoWayMergedDocumentLine>> t2 = GetTwoWayMergeResultAsync(parentPath, rightChildPath);

            await Task.WhenAll(new[] { t1, t2 });

            leftResult = t1.Result;
            rightResult = t2.Result;

            result = CreateThreeWayMergeFile(leftResult, rightResult);

            return result;
        }

        private async Task<List<TwoWayMergedDocumentLine>> GetTwoWayMergeResultAsync(string parentPath, string childPath)
        {
            return await Task.Run(() => GetMergeResult(parentPath, childPath));
        }
        private List<TwoWayMergedDocumentLine> GetMergeResult(string parentFilePath, string childFilePath)
        {
            List<TwoWayMergedDocumentLine> result = new List<TwoWayMergedDocumentLine>();
            string parentLine;
            string childLine;
            try
            {
                using (StreamReader childReader = new StreamReader(childFilePath, Encoding.Default))
                {
                    while ((childLine = childReader.ReadLine()) != null)
                    {
                        result.Add(new TwoWayMergedDocumentLine()
                        {
                            Text = childLine,
                            State = TwoWayMergeDocumentLineState.None
                        });
                    }
                }

                using (StreamReader parentReader = new StreamReader(parentFilePath, Encoding.Default))
                {
                    int childCurrentIndex = 0;
                    int lineNumber = 0;

                    while ((parentLine = parentReader.ReadLine()) != null)
                    {
                        lineNumber++;

                        bool lineFound = false;

                        for (int i = childCurrentIndex; i < result.Count; i++)
                        {
                            if (result[i].Text.Trim().Equals(parentLine.Trim()))
                            {
                                result[i].ParentDocumentLineNumer = lineNumber;
                                result[i].State = TwoWayMergeDocumentLineState.Equal;

                                childCurrentIndex = i;
                                lineFound = true;

                                break;
                            }

                            if (result[i].Text.Trim().IsSimilarTo(parentLine.Trim()))
                            {
                                result[i].ParentDocumentLineNumer = lineNumber;
                                result[i].Text = result[i].Text;
                                result[i].State = TwoWayMergeDocumentLineState.Modified;

                                childCurrentIndex = i;
                                lineFound = true;

                                break;
                            }

                            result[i].State = TwoWayMergeDocumentLineState.New;
                            continue;
                        }

                        if (!lineFound)
                        {
                            result.Insert(childCurrentIndex, new TwoWayMergedDocumentLine()
                            {
                                Text = parentLine,
                                ParentDocumentLineNumer = lineNumber,
                                State = TwoWayMergeDocumentLineState.Removed
                            });
                        }


                        childCurrentIndex++;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private List<ThreeWayMergedDocumentLine> CreateThreeWayMergeFile(List<TwoWayMergedDocumentLine> leftChildMerge, List<TwoWayMergedDocumentLine> rightChildMerge)
        {
            List<ThreeWayMergedDocumentLine> result = new List<ThreeWayMergedDocumentLine>();
            int leftIndex = 0, rightIndex = 0;

            for (; leftIndex < leftChildMerge.Count && rightIndex < rightChildMerge.Count;)
            {
                if(leftChildMerge[leftIndex].State == TwoWayMergeDocumentLineState.New)
                {
                    result.Add(new ThreeWayMergedDocumentLine()
                    {
                        State = ThreeWayMergeDocumentLineState.New,
                        Text = leftChildMerge[leftIndex].Text,
                        Parent = PatentType.Left
                    });

                    leftIndex++;
                    continue;
                }

                if(rightChildMerge[rightIndex].State == TwoWayMergeDocumentLineState.New)
                {
                    result.Add(new ThreeWayMergedDocumentLine()
                    {
                        State = ThreeWayMergeDocumentLineState.New,
                        Text = rightChildMerge[rightIndex].Text,
                        Parent = PatentType.Right
                    });

                    rightIndex++;
                    continue;
                }

                ThreeWayMergedDocumentLine lineToAdd = new ThreeWayMergedDocumentLine();
                ThreeWayMergedDocumentLine conflictLine = null;

                switch (leftChildMerge[leftIndex].State)
                {
                    case TwoWayMergeDocumentLineState.Equal:
                        switch (rightChildMerge[rightIndex].State)
                        {
                            case TwoWayMergeDocumentLineState.Equal:
                                lineToAdd.Text = leftChildMerge[leftIndex].Text;
                                lineToAdd.State = ThreeWayMergeDocumentLineState.Equal;
                                lineToAdd.Parent = PatentType.Parent;
                                break;
                            case TwoWayMergeDocumentLineState.Removed:
                                lineToAdd.Text = leftChildMerge[leftIndex].Text;
                                lineToAdd.State = ThreeWayMergeDocumentLineState.Removed;
                                lineToAdd.Parent = PatentType.Right;
                                break;
                            case TwoWayMergeDocumentLineState.Modified:
                                lineToAdd.Text = rightChildMerge[rightIndex].Text;
                                lineToAdd.State = ThreeWayMergeDocumentLineState.Modified;
                                lineToAdd.Parent = PatentType.Right;
                                break;
                        }

                        break;
                    case TwoWayMergeDocumentLineState.Removed:
                        switch (rightChildMerge[rightIndex].State)
                        {
                            case TwoWayMergeDocumentLineState.Equal:
                                lineToAdd.Text = leftChildMerge[leftIndex].Text;
                                lineToAdd.State = ThreeWayMergeDocumentLineState.Removed;
                                lineToAdd.Parent = PatentType.Left;
                                break;
                            case TwoWayMergeDocumentLineState.Removed:
                                lineToAdd.Text = leftChildMerge[leftIndex].Text;
                                lineToAdd.State = ThreeWayMergeDocumentLineState.Removed;
                                lineToAdd.Parent = PatentType.Left;
                                break;
                            case TwoWayMergeDocumentLineState.Modified:
                                lineToAdd.Text = "";
                                lineToAdd.State = ThreeWayMergeDocumentLineState.Conflict;
                                lineToAdd.Parent = PatentType.Left;

                                conflictLine = new ThreeWayMergedDocumentLine()
                                {
                                    Text = rightChildMerge[rightIndex].Text,
                                    Parent = PatentType.Right,
                                    State = ThreeWayMergeDocumentLineState.Conflict
                                };

                                break;
                        }

                        break;
                    case TwoWayMergeDocumentLineState.Modified:
                        switch (rightChildMerge[rightIndex].State)
                        {
                            case TwoWayMergeDocumentLineState.Equal:
                                lineToAdd.Text = leftChildMerge[leftIndex].Text;
                                lineToAdd.State = ThreeWayMergeDocumentLineState.Modified;
                                lineToAdd.Parent = PatentType.Left;
                                break;
                            case TwoWayMergeDocumentLineState.Removed:
                                lineToAdd.Text = leftChildMerge[leftIndex].Text;
                                lineToAdd.State = ThreeWayMergeDocumentLineState.Conflict;
                                lineToAdd.Parent = PatentType.Left;

                                conflictLine = new ThreeWayMergedDocumentLine()
                                {
                                    Text = "",
                                    Parent = PatentType.Right,
                                    State = ThreeWayMergeDocumentLineState.Conflict
                                };

                                break;
                            case TwoWayMergeDocumentLineState.Modified:
                                if (leftChildMerge[leftIndex].Text.Trim() == rightChildMerge[rightIndex].Text.Trim())
                                {
                                    lineToAdd.Text = leftChildMerge[leftIndex].Text;
                                    lineToAdd.State = ThreeWayMergeDocumentLineState.Modified;
                                    lineToAdd.Parent = PatentType.Left;
                                }
                                else
                                {
                                    lineToAdd.Text = leftChildMerge[leftIndex].Text;
                                    lineToAdd.State = ThreeWayMergeDocumentLineState.Conflict;
                                    lineToAdd.Parent = PatentType.Left;

                                    conflictLine = new ThreeWayMergedDocumentLine()
                                    {
                                        Text = rightChildMerge[rightIndex].Text,
                                        Parent = PatentType.Right,
                                        State = ThreeWayMergeDocumentLineState.Conflict
                                    };
                                }
                                break;
                        }

                        break;
                }

                result.Add(lineToAdd);
                if(conflictLine != null)
                {
                    result.Add(conflictLine);
                }

                leftIndex++;
                rightIndex++;
            }


            for (; leftIndex < leftChildMerge.Count; leftIndex++)
            {
                result.Add(new ThreeWayMergedDocumentLine() {
                    Text = leftChildMerge[leftIndex].Text,
                    Parent = PatentType.Left,
                    State = (ThreeWayMergeDocumentLineState)leftChildMerge[leftIndex].State
                });
            }

            for (; rightIndex < rightChildMerge.Count; rightIndex++)
            {
                result.Add(new ThreeWayMergedDocumentLine()
                {
                    Text = rightChildMerge[rightIndex].Text,
                    Parent = PatentType.Right,
                    State = (ThreeWayMergeDocumentLineState)rightChildMerge[rightIndex].State
                });
            }

            return result;
        }


    }
}
