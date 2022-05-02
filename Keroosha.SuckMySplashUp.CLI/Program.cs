// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using Keroosha.SuckMySplashUp.Patcher;

var appOption = new Option<string>(
    "--app",
    "Path to app.jar of your IDE to scan splash screen images");
var splashOption = new Option<string>(
    "--splash",
    "path to splash screen png file");
var splashx2 = new Option<string>(
    "--splash2",
    "path to splash screen x2 png file");
var rootCommand = new RootCommand(description: "Patches app.jar with user defined splash screens")
{
    appOption,
    splashOption,
    splashx2
};
var handler = (string app, string splash, string splash2) => new Patcher(app, splash, splash2).PatchJar();

rootCommand.SetHandler(handler, appOption, splashOption, splashx2);
rootCommand.Invoke(args);