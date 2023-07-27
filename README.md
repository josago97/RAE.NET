# Diccionario Real Academia Española API

[![license](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/josago97/RAE.NET/blob/master/LICENSE) [![NuGet version (RAE.NET)](https://img.shields.io/nuget/v/RAE.NET.svg)](https://www.nuget.org/packages/RAE.NET/) [![Downloads](https://img.shields.io/nuget/dt/RAE.NET.svg)](https://www.nuget.org/packages/RAE.NET/)

Esta es una biblioteca no oficial que te permite:
<br>
This is an unofficial library that allows you to:


- Obtener definiciones de una palabra. 
<br>
Get definitions of a word.

```cs
DRAE drae = new DRAE();
IWord[] words = await drae.FetchWordAsync("casa");

foreach (IWord word in words)
{
    Console.WriteLine(word);
}
```
<br>

Obtener definiciones de una palabra por identificador. 
<br>
Get definitions of a word by id.

```cs
DRAE drae = new DRAE();
IWord word = await drae.FetchWordByIdAsync("7lsKMtR");

Console.WriteLine(word);
```
<br>

Obtener la palabra del día. 
<br>
Get the word of the day.

```cs
DRAE drae = new DRAE();
IEntry entry = await drae.GetWordOfTheDayAsync();

Console.WriteLine(entry);
```
<br>

Obtener una palabra aleatoria. 
<br>
Get a random word.

```cs
DRAE drae = new DRAE();
IWord word = await drae.GetRandomWordAsync();

Console.WriteLine(word);
```
<br>

Buscar las entradas de una palabra.
<br>
Search the entries of a word.

```cs
DRAE drae = new DRAE();
IEntry[] entries = await drae.SearchWordAsync("y");

Console.WriteLine(string.Join(", ", entries));
```
<br>

Buscar palabras clave.
<br>
Search for keywords.

```cs
DRAE drae = new DRAE();
string[] keys = await drae.GetKeysAsync("hola");

Console.WriteLine(string.Join(", ", keys));
```
<br>

Obtener todas las palabras que comienzan con una cadena. 
<br>
Get all words that start with a string.

```cs
DRAE drae = new DRAE();
string[] words = await drae.GetWordsStartWithAsync("sa");

Console.WriteLine(string.Join(", ", words));
```
<br>

Obtener todas las palabras que continenen una cadena. 
<br>
Get all words that contain a string.

```cs
DRAE drae = new DRAE();
string[] words = await drae.GetWordsContainAsync("sa");

Console.WriteLine(string.Join(", ", words));
```
<br>

Obtener todas las palabras del diccionario. 
<br>
Get all word of the dictionary.

```cs
DRAE drae = new DRAE();
string[] allWords = await drae.GetAllWordsAsync();

Console.WriteLine(string.Join(", ", allWords));
```
