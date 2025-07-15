using System;
using System.Collections.Generic;

[Serializable]
public class Soal
{
    public string soal;
    public string pilihanA;
    public string pilihanB;
    public string pilihanC;
    public string pilihanD;
    public string jawaban; // A/B/C/D
}

[Serializable]
public class SoalWrapper
{
    public List<Soal> list;
}