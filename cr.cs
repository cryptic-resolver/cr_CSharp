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


// cannot be const
string CRYPTIC_RESOLVER_HOME = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

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

string bold(string str)       { return String.Format("\x1b[1m{0}\x1b[0m",str); }
string underline(string str)  { return String.Format("\x1b[4m${0}\x1b[0m",str); }
string red(string str)        { return String.Format("\x1b[31m${0}\x1b[0m",str); }
string green(string str)      { return String.Format("\x1b[32m${0}\x1b[0m",str); }
string yellow(string str)     { return String.Format("\x1b[33m${0}\x1b[0m",str); }
string blue(string str)       { return String.Format("\x1b[34m${0}\x1b[0m",str); }
string purple(string str)     { return String.Format("\x1b[35m${0}\x1b[0m",str); }
string cyan(string str)       { return String.Format("\x1b[36m${0}\x1b[0m",str); }


Console.WriteLine(bold("haha"));
Console.WriteLine(underline("haha"));
Console.WriteLine(red("haha"));
Console.WriteLine(blue("haha"));
Console.WriteLine(purple("haha"));


void add_default_sheet_if_none_exist(){
    Console.WriteLine("TODO: add default sheet");
}


void update_sheets(string sheet_repo){
    Console.WriteLine("TODO: update sheets");
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



// no need to write
// static void Main(string args)
// and args are already in this global scope

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


