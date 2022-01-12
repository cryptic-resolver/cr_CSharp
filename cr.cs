//   ---------------------------------------------------
//   File          : cr.cs
//   Authors       : ccmywish <ccmywish@qq.com>
//   Created on    : <2021-1-12>
//   Last modified : <2022-1-12>
//
//   This file is used to explain a CRyptic command
//   or an acronym's real meaning in computer world or
//   orther fileds.
//
//  ---------------------------------------------------


// We don't use it for color anymore
// using Pastel;
// using System.Drawing;   // For `Color` class

using Carbon.Toml;

// cannot be const
string CRYPTIC_RESOLVER_HOME = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.cryptic-resolver";

// or use var
Dictionary<string,string> CRYPTIC_DEFAULT_SHEETS = new Dictionary<string, string> {
	["computer"]= "https://github.com/cryptic-resolver/cryptic_computer.git",
	["common"]=   "https://github.com/cryptic-resolver/cryptic_common.git",
	["science"]=  "https://github.com/cryptic-resolver/cryptic_science.git",
	["economy"]=  "https://github.com/cryptic-resolver/cryptic_economy.git",
	["medicine"]= "https://github.com/cryptic-resolver/cryptic_medicine.git"
};

const string CRYPTIC_VERSION = "1.0.0";

//
// helper: for color
// 
// It's same as in NodeJS

// string bold(string str)       { return String.Format("\x1b[1m{0}\x1b[0m",str); }
string underline(string str)  { return String.Format("\x1b[4m${0}\x1b[0m",str); }
string red(string str)        { return String.Format("\x1b[31m${0}\x1b[0m",str); }
string green(string str)      { return String.Format("\x1b[32m${0}\x1b[0m",str); }
// string yellow(string str)     { return String.Format("\x1b[33m${0}\x1b[0m",str); }
// string blue(string str)       { return String.Format("\x1b[34m${0}\x1b[0m",str); }
string purple(string str)     { return String.Format("\x1b[35m${0}\x1b[0m",str); }
// string cyan(string str)       { return String.Format("\x1b[36m${0}\x1b[0m",str); }


//
// core: logic
//

bool is_there_any_sheet() {

	string path = CRYPTIC_RESOLVER_HOME;

	if (! Directory.Exists(path)) {
		Directory.CreateDirectory(path);
	}
	var dirnum = Directory.GetDirectories(path).Length;

	if (dirnum == 0)
		return false;
	else 
		return true;
}





void add_default_sheet_if_none_exist(){

    if (!is_there_any_sheet()) {
		Console.WriteLine("cr: Adding default sheets...");

		foreach(var (key, value) in CRYPTIC_DEFAULT_SHEETS) {
			Console.WriteLine("cr: Pulling cryptic_" + key + "...");
            
            string command = $"-C {CRYPTIC_RESOLVER_HOME} clone {value} -q";
            var proc = System.Diagnostics.Process.Start("git", command);    // no need to use "cmd" /C git ...." 
            // must wait
            proc.WaitForExit();
            var exitCode = proc.ExitCode;
            if (exitCode!=0){
                // Read the standard error and write it on to console.
                // Console.WriteLine("Pull failed!");
                // Git will automatically output its error
            }
		}

		Console.WriteLine("cr: Add done");
	}

}


void update_sheets(string sheet_repo){

	add_default_sheet_if_none_exist();

	if(sheet_repo == "") {
		Console.WriteLine("cr: Updating all sheets...");

		var dirs = Directory.GetDirectories(CRYPTIC_RESOLVER_HOME);

		foreach(var dir in dirs) {
            // path to dirname
			string sheet = Path.GetFileName(dir);   // This api name is shit
			Console.WriteLine("cr: Wait to update {0}...", sheet);
            string command = $"-C  {CRYPTIC_RESOLVER_HOME}/{sheet} pull -q";
			var proc = System.Diagnostics.Process.Start("git",command);
			proc.WaitForExit();
		}
		Console.WriteLine("cr: Update done");
	
	} else {
		Console.WriteLine("cr: Adding new sheet...");
        string command = $"-C ${CRYPTIC_RESOLVER_HOME} clone {sheet_repo} -q";
		var proc = System.Diagnostics.Process.Start("git", command);
		proc.WaitForExit();
		Console.WriteLine("cr: Add new sheet done");
	}


}


// path: sheet name, eg. cryptic_computer
// file: dict(file) name, eg. a,b,c,d
// dict: the concrete dict
// 		 var dict map[string]interface{}
// 
// We can't use pointer of an managed type
Carbon.Json.JsonObject load_dictionary(string path, string file) {

	string toml_file = $"{CRYPTIC_RESOLVER_HOME}/{path}/{file}.toml";

	if (! File.Exists(toml_file)) {
		var emptyDoc = new Carbon.Json.JsonObject{}; // return null will invoke a warning when built
        return emptyDoc;
	} else {
        string str = File.ReadAllText(toml_file);
		var doc = Toml.Parse(str);
		return doc;
	}

}




// Pretty print the info of the given word
void pp_info(Carbon.Json.JsonNode infonode){

    var info = infonode.As<Carbon.Json.JsonObject>();
	// We should convert disp, desc, full into string

	// can't directly cast TOMLValue to string
	string disp;
	if (info.ContainsKey("disp")){
		disp = (string)info["disp"];
	}else {
        disp = red("No name!");
	}

	Console.WriteLine("\n  {0}: {1}", disp, (string)info["desc"]);

    if (info.ContainsKey("full")){
		Console.WriteLine("\n  {0}", (string)info["full"]);
	}

	// see is string[]
    if (info.ContainsKey("see")){
		string[] see = info["see"].ToArrayOf<string>(); // Notice here

		Console.Write("\n{0} ", purple("SEE ALSO "));

		foreach(var val in see) {
			Console.Write(underline(val) );
		}

		Console.WriteLine();
	}
	Console.WriteLine();
}


// Print default cryptic_ sheets
void pp_sheet(string sheet) {
	Console.WriteLine(green("From: " + sheet));
}




//  Used for synonym jump
//  Because we absolutely jump to a must-have word
//  So we can directly lookup to it
//
//  Notice that, we must jump to a specific word definition
//  So in the toml file, you must specify the precise word.
//  If it has multiple meanings, for example
//
//    [blah]
//    same = "XDG"  # this is wrong
//
//    [blah]
//    same = "XDG.Download" # this is right
bool directly_lookup(string sheet, string file, string word) {

	Carbon.Json.JsonObject dict;

	dict = load_dictionary(sheet, file.ToLower()); // std.string: toLower

	if(dict == new Carbon.Json.JsonObject(){}) {
		Console.WriteLine("WARN: Synonym jumps to a wrong place");
		Environment.Exit(0);
	}

	string[] words = word.Split("."); // [XDG Download]
	string dictword = words[0].ToLower();       // XDG [Download]
    
    Carbon.Json.JsonNode info;

	if (words.Length == 1) { // [HEHE]
		info = dict[dictword];

	} else { //  [XDG Download]
		string explain = words[1];
		Carbon.Json.JsonNode indirect_info = dict[dictword];
		info = indirect_info[explain];
	}

	// Warn user this is the toml maintainer's fault
	// the info map is empty
	if (info == null) {
		string str = @"WARN: Synonym jumps to a wrong place at `%s` \n 
	Please consider fixing this in `%s.toml` of the sheet `%s`";

		string redstr = red(String.Format(str, word, file.ToLower(), sheet));

		Console.WriteLine(redstr);
		Environment.Exit(0); // not portable??
	}

	pp_info(info);
	return true; // always true
}








void solve_word(string word_2_solve){
    Console.WriteLine("TODO: solve word");
}


void help() 
{
    string help = @"cr: Cryptic Resolver version {0} in C#

usage:
    cr -h                     => print this help
    cr -u (xx.com//repo.git)  => update default sheet or add sheet from a git repo
    cr emacs                  => Edit macros: a feature-rich editor";
	
    Console.WriteLine(help, CRYPTIC_VERSION);
}



// No need to write
// static void Main(string args)
// and args are already in this Top-level environment
// And the project should only has this file as entry point

string arg;	
int arg_num = args.Length;	// can't be args.length

if(arg_num < 1) {
	arg = "";
} else {
	arg = args[0];
}

// Console.WriteLine($"{arg_num} {arg}"); // DEBUG

switch (arg) {
case "":
	help();
	add_default_sheet_if_none_exist();
	break;
case "-h":
	help();
	break;
case "-u":
	if (arg_num > 1) {
		update_sheets(args[1]);
	} else {
		update_sheets("");
	}
	break;
default:
	solve_word(arg);
    break;  // need this to make compiler happy
}


