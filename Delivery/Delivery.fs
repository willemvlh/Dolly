﻿module Dolly.Actions

open System.IO
open System.Windows.Forms
open System.Xml.Linq
open System.Xml
open System.Collections.Generic
open System.Text.RegularExpressions
open WK.Libraries.BetterFolderBrowserNS

let chooseFolderWithRootFolder dialogTitle rootFolder =
    let d = new BetterFolderBrowser()
    d.RootFolder <- rootFolder
    d.Title <- dialogTitle
    if d.ShowDialog() = DialogResult.OK then d.SelectedFolder
    else exit -1

let chooseFolder dialogTitle = chooseFolderWithRootFolder dialogTitle ""
    
let copySingleFile (destinationDir: string) (from: string) =
    let destination = Path.Combine(destinationDir, Path.GetFileName(from))
    File.Copy(from, destination, true)

let copyAllFiles (fromDir: string) (destinationDir: string) = 
    Directory.GetFiles(fromDir) |> Seq.iter (copySingleFile destinationDir)

let copyFolderTo (folder: string) (destination: string) =
    let dirName = Path.GetFileName(folder)
    let combined = Path.Combine(destination, dirName)
    Directory.CreateDirectory(combined).FullName |> copyAllFiles folder
    combined

let findReportDefinitions (folder: string) =
    let pattern = ".*(def|report).*\.xml"
    let isReportDef (file : string) = Regex.IsMatch(file, pattern, RegexOptions.IgnoreCase)
    try Directory.GetFiles(folder) |> Seq.find isReportDef
    with 
    | :? KeyNotFoundException -> failwithf "No report definition found. The report definition must match the pattern %s" pattern   
    | :? System.ArgumentNullException -> failwithf "The folder (%s) is empty." folder

let writeDocument (location: string) (document: XDocument) =
    let settings = XmlWriterSettings()
    settings.Indent <- true
    use writer = XmlWriter.Create(location, settings)
    document.WriteTo(writer)

