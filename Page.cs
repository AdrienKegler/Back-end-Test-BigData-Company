using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;


namespace Back_end_Test_BigData_Company
{
    public class Page
    {
        // Properties.
        public string url { get; set; }
        public uint depth { get; set; }
        private String htmlCode { get; set; }
        private uint paragraphSize { get; set; }
        public List<Page> childrenList { get; set; }


        // Constructor.
        public Page(String url, uint depth = 0)
        {
            childrenList = new List<Page>();
            this.url = url;
            this.depth = depth;
            // this.htmlCode = "<!DOCTYPE html><html><head><title>This is the title</title></head><body><a href=\"link 1\"></a><a href=\"link 2\"></a>	<a href=\"link 3\"></a><a href=\"link 4\"></a><a href=\"link 5\"></a><a href=\"link 6\"></a><p>Text 1</p><p>Text 2</p><p>Text 3</p><p>Text 4</p><p>Text 5</p><p>Text 6</p><p>Text 7</p></body></html>";
            this.htmlCode = this.getHTMLCode();
            setParagraphSize();
        }



        // Methods

        private String getHTMLCode()
        {
            String urlAddress = this.url;
            String data = "";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();



                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (response.CharacterSet == null)
                    {
                        // default encoding is UTF8
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        String charset = "";
                        switch (response.CharacterSet)
                        {
                            case "\"utf-8\"": // this was the value of response.CharacterSet as tested on a specific website. Here is a dirty fix.
                                charset = "utf-8";
                                break;
                            default:
                                charset = "utf-8";
                                break;
                        }

                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(charset));
                    }

                    data = readStream.ReadToEnd();

                    response.Close();
                    readStream.Close();
                }
            }
            catch { }
            return data;
        }

        private void setParagraphSize()
        {
            MatchCollection matches = Regex.Matches(this.htmlCode, "<p.*?>(.*?)</p>");

            Console.WriteLine("Here is the list of page content found on " + this.url);

            String paragraphsChain = "";

            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    String paragraph = m.Groups[1].Value;
                    Console.WriteLine(paragraph);
                    paragraphsChain += paragraph;
                    // removing all whitespaces
                    paragraphsChain = Regex.Replace(paragraphsChain, @"\s+", "");
                    this.paragraphSize = (uint)paragraphsChain.ToCharArray().Length;
                }
            }
            else
            {
                Console.WriteLine("Nothing found");
            }
        }

        public void dig(uint maxDepth)
        {
            if (this.depth < maxDepth)
            {
                this.setChildrenList();
                if (this.depth < maxDepth - 1)
                {
                    foreach (Page children in this.childrenList)
                    {
                        children.dig(maxDepth);
                    }
                }
            }
        }

        private void setChildrenList()
        {
            // Some space between each Pages so it's prettier
            for (int i = 0; i < 3; i++)
                Console.WriteLine("");

            MatchCollection matches = Regex.Matches(this.htmlCode, "(<a.*?>.*?</a>)");

            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    String tag = m.Groups[0].Value;
                    // Matches the simple/double quote just after href then matches the following chars until meeting the string delimiter (simple/double quote LIKE the first match)
                    String link = Regex.Match(tag, @"<a.*?href=([""'])(.*?)\1").Groups[2].Value;
                    if (!link.StartsWith("http")) // handeling relative paths
                    {
                        String domainName = this.url.EndsWith("/") ? this.url.Remove(this.url.Length - 1) : this.url;
                        domainName += "/";
                        switch (link.ToCharArray()[0])
                        {
                            case '/':
                                link.Remove(0, 1);
                                break;
                            case '#':
                                domainName.Remove(0, 1);
                                break;
                            default:
                                break;
                        }
                        link = domainName + link;
                    }
                    Console.WriteLine(link + " has been discovered from " + this.url);
                    Page newChildren = new Page(link, this.depth + 1);
                    this.childrenList.Add(newChildren);

                    // Some space between each Pages so it's prettier
                    for (int i = 0; i < 3; i++)
                        Console.WriteLine("");
                }
            }
        }

        public uint getParagraphSizeRecurs()
        {
            uint totalParagraphSize = this.paragraphSize;

            foreach (Page child in childrenList)
            {
                totalParagraphSize += child.getParagraphSizeRecurs();
            }

            return totalParagraphSize;
        }
    }
}