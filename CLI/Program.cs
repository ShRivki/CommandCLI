
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

var bundeleOptionLanguage = new Option<String>("--language");
    bundeleOptionLanguage.AddAlias("-l");
    bundeleOptionLanguage.SetDefaultValue("all");
var bundeleOptionOutput = new Option<FileInfo>("--output");
    bundeleOptionOutput.AddAlias("-o");
var bundeleOptionNote = new Option<bool>("--note");
    bundeleOptionNote.AddAlias("-n");
    bundeleOptionNote.SetDefaultValue(false);
var bundeleOptionSort = new Option<string>("--sort");
    bundeleOptionSort.AddAlias("-s");
    bundeleOptionSort.SetDefaultValue("AB");

var bundeleOptionRemove_empty_lines = new Option<bool>("--remove-empty-lines");
    bundeleOptionRemove_empty_lines.AddAlias("-r");
    bundeleOptionRemove_empty_lines.SetDefaultValue(false);

var bundeleOptionAuthor = new Option<String>("--author");
    bundeleOptionAuthor.SetDefaultValue(null);
    bundeleOptionAuthor.AddAlias("-a");


var bundelCommand = new Command("bundel", "bundel code files to a singel file");
bundelCommand.AddAlias("b");

bundelCommand.SetHandler((output, note, Language, sort, remove, name) =>
{
    string[] l = Language.Split(",");
    var projectDirectory = Directory.GetCurrentDirectory();
    List<string> codeFiles = new List<string>();
    //if(!sort.Equals("AB"))
    //{
    //   Array.Sort(l);
    //}
    if (Language.Contains("all"))
    {
        codeFiles = Directory.GetFiles(projectDirectory, "*.*", SearchOption.AllDirectories).Where(file => !file.Contains("\\bin\\") && !file.Contains("\\Debug\\"))
                            .Where(file => !Path.GetExtension(file).Equals(".exe", StringComparison.OrdinalIgnoreCase) &&
                                           !Path.GetExtension(file).Equals(".dll", StringComparison.OrdinalIgnoreCase)).ToList();
        Console.WriteLine("allllllllll");
    }
    else
    {
        foreach (string extension in l)
        {          
            List<string> filesWithExtension = Directory.GetFiles(projectDirectory, $"*.{extension}", SearchOption.AllDirectories)
            .Where(file => !file.Contains("\\bin\\") && !file.Contains("\\Debug\\"))
            .ToList();
            codeFiles.AddRange(filesWithExtension);
        }    

    }
    if(sort != null && sort == "AB")
    {
        codeFiles = codeFiles.OrderBy(x => Path.GetFileName(x)).ToList();
    }
    else
        codeFiles = codeFiles.OrderBy(x => Path.GetExtension(x)).ToList();
    try
    {
        StreamWriter writer = new StreamWriter(output.FullName+".txt");
        Console.WriteLine("create File ");

        try
        {
            if (name != null)
            {
                writer.WriteLine("//Author:" + name);
            }
            if (remove)
            {
                foreach (var code in codeFiles)
                {                
                    StreamReader file = new StreamReader(code);
                    string line= file.ReadLine();
                    while (line!= null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            writer.WriteLine(line);                           
                        }
                        line = file.ReadLine();
                    }
                }
            }
            else
            {
                foreach (var code in codeFiles)
                {
                    string fileContent = File.ReadAllText(code);
                    writer.WriteLine(fileContent);
                    writer.WriteLine();
                }
            }        
            writer.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("not to exsees to write to file");
        }


        if (note == true)
        {
            try
            {
                int x = output.FullName.LastIndexOf("\\");
                string s = "";
                string p = "";
                if (x != -1)
                {
                    s = output.FullName.Substring(0, x + 1);
                    p = output.FullName.Substring(x + 1);
                }
                else
                {
                    p = output.FullName;
                }
                StreamWriter writer2 = new StreamWriter(s + "source code-" + p+".txt");
                writer2.WriteLine("The source of the file:");
                writer2.WriteLine(projectDirectory);
                x = projectDirectory.LastIndexOf("\\");
                p = projectDirectory.Substring(x + 1);
                writer2.WriteLine("name of fill:"+p);
                Console.WriteLine("create note");
                writer2.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("error:  note cannot be created");
            }
        }

    }
    catch (DirectoryNotFoundException e)
    {
        Console.WriteLine("File path invalide--");
    }


}, bundeleOptionOutput, bundeleOptionNote, bundeleOptionLanguage, bundeleOptionSort, bundeleOptionRemove_empty_lines, bundeleOptionAuthor);

var create_rspCommand = new Command("create-rsp");
create_rspCommand.AddAlias("c");
create_rspCommand.SetHandler(() =>
{
    Console.WriteLine("Enter file routing");
    FileInfo s = new FileInfo(Console.ReadLine());
    Console.WriteLine("Insert the desired languages ​​in this format C#, HTML..\r\nTo select all languages ​​enter all");
    string s2 = Console.ReadLine();
    Console.WriteLine("Whether to list the source code as a comment in the file Y\\N ");
    bool c = char.Parse(Console.ReadLine())=='y'?  true:false;
    Console.WriteLine("Save in order A B  Y\\N");
    string c2 = char.Parse(Console.ReadLine()) == 'y' ? "AB" : "ending";
    Console.WriteLine("Do delete empty lines y/n");
    bool c3 = char.Parse(Console.ReadLine()) == 'y' ? true : false;
    Console.WriteLine("Insert editor name");
    string name = Console.ReadLine();
    Console.WriteLine("Enter a name for response files");
    string nameOfResponseFiles = Console.ReadLine();
    var projectDirectory = Directory.GetCurrentDirectory();
    StreamWriter writer2 = new StreamWriter(projectDirectory+"\\"+nameOfResponseFiles+ ".txt");
    writer2.WriteLine($" b -o {s.FullName} -l {s2} -n {c} -a {name} -s {c2} -r {c3}");
    Console.WriteLine("create ResponseFiles");
    writer2.Close();

});
var rootCommand = new RootCommand("Root command File Bundler CLI");
rootCommand.AddCommand(bundelCommand);
rootCommand.AddCommand(create_rspCommand);
bundelCommand.AddOption(bundeleOptionOutput);
bundelCommand.AddOption(bundeleOptionRemove_empty_lines);
bundelCommand.AddOption(bundeleOptionAuthor);
bundelCommand.AddOption(bundeleOptionLanguage);
bundelCommand.AddOption(bundeleOptionNote);
bundelCommand.AddOption(bundeleOptionSort);
rootCommand.InvokeAsync(args);
//dotnet publish -o publish
