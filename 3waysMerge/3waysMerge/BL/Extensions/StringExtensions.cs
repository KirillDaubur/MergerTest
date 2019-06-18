using System;
using System.Collections.Generic;
using System.Text;

namespace _3waysMerge.BL.Extensions
{
    public static class StringExtensions
    {
        private static float similarityBorder = 0.2f;

        public static bool IsSimilarTo(this string left, string right)
        {
            char[] splitSymbols = new char[] { ' ', ',', '.', ';' };           //, ':', '{', '}', '[', ']', '?', '/', '\\', '(', ')', '<', '>', '\'', '"'

            string[] leftWords = left.Split(splitSymbols);
            string[] rightWords = right.Split(splitSymbols);

            int leftIndex = 0, rightIndex = 0;
            int mismatchWordsCount = 0;

            for (; leftIndex < leftWords.Length; leftIndex++)
            {
                int mismatchResult = 0;
                bool wordFound = false;

                for (int i = rightIndex; i < rightWords.Length; i++)
                {
                    if (rightWords[i].Trim().Equals(leftWords[leftIndex].Trim()))
                    {
                        rightIndex = i;
                        wordFound = true;

                        break;
                    }
                    else
                    {
                        mismatchResult++;
                    }

                    continue;
                }

                if (wordFound)
                {
                    mismatchWordsCount += mismatchResult;
                }
                else
                {
                    mismatchWordsCount++;
                }

                rightIndex++;
            }

            mismatchWordsCount += leftWords.Length - leftIndex;
            mismatchWordsCount += rightWords.Length - rightIndex;

            return mismatchWordsCount < similarityBorder * leftWords.Length;
        }
    }
}
