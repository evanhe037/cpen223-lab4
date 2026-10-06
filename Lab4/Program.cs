// Lab 4
// Student name: Evan He
// Student number: 82093519

using System;
using System.Collections.Generic;

string userName = "Evan He";
Console.WriteLine($"CPEN223 Lab 2 for user: {userName}");

Dictionary<string, int> counts = GenomeAnalyzer.CountKMers("AAAA", 2);
Console.WriteLine($"Test 1 - CountKMers:");
Console.WriteLine($"Expected: AA -> 3 (1 entry), Actual: AA -> {counts["AA"]} ({counts.Count} entries)");
// testing for CountKMers

Dictionary<string, int> diffs = GenomeAnalyzer.CompareProfiles("ACGACG", "ACGTCG", 3);
Console.WriteLine($"\nTest 2 - CompareProfiles:");
foreach (KeyValuePair<string, int> kvp in diffs)
{
    Console.WriteLine($"k-mer: {kvp.Key}, Change: {kvp.Value}");
}
// testing for CompareProfiles

List<string> mostChanged = GenomeAnalyzer.MostChangedKMers("AAAA", "TTTT", 2);
Console.WriteLine($"\nTest 3 - MostChangedKMers:");
Console.WriteLine($"Expected count: 2 (AA and TT), Actual count: {mostChanged.Count}");
// testing for MostChangedKMers

bool differ = GenomeAnalyzer.SamplesDiffer("AAAA", "TTTT", 2, 3);
Console.WriteLine($"\nTest 4 - SamplesDiffer:");
Console.WriteLine($"Expected: True, Actual: {differ}");
// testing for SamplesDiffer

public static class GenomeAnalyzer
{

    private static bool IsValidDnaSequence(string sequence)
    {
        if(sequence == null)
        {
            return false;
        }

        foreach(char c in sequence)
        {
            if(c != 'A' && c != 'C' && c != 'G' && c != 'T')
            {
                return false;
            }
        }
        return true;
        // helper to check whether a string has only A, C, G, or T
        // false if null or contains any other characters
    }

    public static Dictionary<string, int> CountKMers(string sequence, int k){
        if (sequence == null)
        {
            throw new ArgumentException("Sequence cannot be null.");
        }
        if (!IsValidDnaSequence(sequence))
        {
            throw new ArgumentException("Sequence has invalid characters, only A, C, G, and T are allowed.");
        }
        if(k <= 0)
        {
            throw new ArgumentException("k must be greater than zero.");
        }
        if(k > sequence.Length)
        {
            throw new ArgumentException("k can't be greater than the length of the sequence.");
        }
        // all if statements to ensure sequence and k are valid

        Dictionary<string, int> profile = new Dictionary<string, int>();
        // creates an empty dictionary with the key as the string and the
        // integers are the values

        for(int i = 0; i <= sequence.Length - k; i++)
        {
            string kmer = sequence.Substring(i, k);
            // loops through to get each kmer of length k starting at the index
            if (profile.ContainsKey(kmer))
            {
                profile[kmer]++;
                // adds to the key if it's another kmer we've already seen
            }
            else
            {
                profile[kmer] = 1;
                // if it's the first time we see it, start at 1
            }
        }
        return profile;
    }
    
    public static Dictionary<string, int> CompareProfiles(string reference, string sample, int k)
    {
        if(reference == null || sample == null)
        {
            throw new ArgumentException("Reference and sample sequences can't be null.");
        }
        if(!IsValidDnaSequence(reference) || !IsValidDnaSequence(sample))
        {
            throw new ArgumentException("Sequences have invalid characters, only A, C, G, and T are allowed.");
        }
        if(k <= 0)
        {
            throw new ArgumentException("k must be greater than zero.");
        }
        if(k > reference.Length || k > sample.Length)
        {
            throw new ArgumentException("k can't be larger than either sequence length.");
        }
        // all if statements to check if sequences are valid

        Dictionary<string, int> refProfile = CountKMers(reference, k);
        Dictionary<string, int> sampleProfile = CountKMers(sample, k);
        // dictionaries from method 1

        Dictionary<string, int> comparison = new Dictionary<string, int>();
        // new dictionary to store new changes

        foreach(KeyValuePair<string, int> kvp in refProfile)
        {
            string kmer = kvp.Key;
            int refCount = kvp.Value;

            sampleProfile.TryGetValue(kmer, out int sampleCount);

            int difference = sampleCount - refCount;
            // checks if kmer exists in reference and sample profile
            // assigns count if found, or sets to 0 if missing
            if(difference != 0)
            {
                comparison[kmer] = difference;
            }
        }

        foreach(KeyValuePair<string, int> kvp in sampleProfile)
        // for kmers not in reference
        {
            string kmer = kvp.Key;

            if (!refProfile.ContainsKey(kmer))
            {
                int sampleCount = kvp.Value;
                comparison[kmer] = sampleCount;
            }
        }
        // difference not computed yet if it wasn't found in reference
        // difference is sampleCount
        return comparison;
    }

    public static List<string> MostChangedKMers(string reference, string sample, int k){
        Dictionary<string, int> diffProfile = CompareProfiles(reference, sample, k);

        List<string> result = new List<string>();
        int maxAbsChange = 0;

        foreach(KeyValuePair<string, int> kvp in diffProfile)
        {
            int absChange = Math.Abs(kvp.Value);
            // accounts for negatives
            if(absChange > maxAbsChange)
            {
                maxAbsChange = absChange;
            }
        }
        // finds the largest absolute change value

        if(maxAbsChange == 0)
        {
            return result;
        }
        // returns empty if identical/empty
        
        foreach(KeyValuePair<string, int> kvp in diffProfile){
            if(Math.Abs(kvp.Value) == maxAbsChange)
            {
                result.Add(kvp.Key);
            }
        }
        return result;
        // collects kmers matching the maximum absolute change
    }

    public static bool SamplesDiffer(string reference, string sample, int k, int threshold)
    {
        if(threshold <= 0)
        {
            throw new ArgumentException("Threshold must be positive.");
        }

        Dictionary<string, int> diffProfile = CompareProfiles(reference, sample, k);

        foreach(KeyValuePair<string, int> kvp in diffProfile)
        {
            if(Math.Abs(kvp.Value) >= threshold)
            {
                return true;
            }
        }
        // checks any single kmer with a change greater than or 
        // equal to the threshold

        return false;
        // no kmer hit the threshold change
    }
}