// Parser for http://gungeongod.com/
var fs = require('fs');
var Crawler = require("crawler");
var format = require('string-format');

function handle(callback) {
    return function(error, res, done) {
        if (error) {
            console.log(error);
        } else {
            callback(res.$);
        }
    }
}

function escapeAndStringifyJson(json) {
    return JSON.stringify(json)
        .replace(/\\/g, '\\\\')
        .replace(/"/g, '\\\"');
}

var c = new Crawler({
    maxConnections: 10
});
c.queue([{
    uri: 'https://enterthegungeon.gamepedia.com/index.php?title=Guns&action=edit&section=1',
    callback: handle(retrieveAllGuns)
}]);

var FILE_FORMAT = "public class NoBrainJsonDB {\n" +
    "    public static readonly string ITEM_JSON = \"{}\";\n" +
    "    public static readonly string SHRINE_JSON = \"{}\";\n" +
    "}";

function retrieveAllGuns($) {
    var getText = function() { return $(this).text() };

    var sanitizeWikiSource = function(wikiSourceString) {
        var lines = wikiSourceString.split(/\r?\n/g);
        lines = lines.slice(5); // cut out the front matter
        lines.forEach(function(line) {
            
        });
        console.log(wikiSourceString);
    };
    
    var gunJsonArray = [];

    var wikiSource = $('#wpTextbox1').text();
    sanitizeWikiSource(wikiSource);
    
    // unfinished here because i got the synergy data at runtime
    
    

    // var fileString = format(FILE_FORMAT,
    //     escapeAndStringifyJson(gunJsonArray),
    //     escapeAndStringifyJson(shrinesJsonArray));
    //
    // fs.writeFile("../src/NoBrainJsonDB.cs", fileString, function(err) {
    //     if(err) {
    //         return console.log(err);
    //     }
    //     console.log("The file was saved!");
    // });
}