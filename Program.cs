using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal class Program
{
    public struct Persona
    {
        public string nome;
        public List<string> esercizi;
        public List<int> carichi;
        public DateTime data;
    }

    static double MiglioramentoPerc(double caricoAttuale, double caricoIniziale)
    {
        return ((caricoAttuale - caricoIniziale) / caricoIniziale) * 100;
    }

    static void inserisciPersone(Persona[] persone, ref int n)
    {
        Console.WriteLine("Quante persone vuoi aggiungere?");
        int daAggiungere = Convert.ToInt32(Console.ReadLine());

        for (int i = 0; i < daAggiungere; i++)
        {
            Persona nuovaPersona = new Persona();
            nuovaPersona.esercizi = new List<string>();
            nuovaPersona.carichi = new List<int>();

            Console.WriteLine("Nome della persona:");
            nuovaPersona.nome = Console.ReadLine();

            Console.WriteLine("Quanti esercizi vuoi registrare?");
            int daregistrare = Convert.ToInt32(Console.ReadLine());
            for (int j = 0; j < daregistrare; j++)
            {
                Console.WriteLine($"Esercizio {j + 1}:");
                nuovaPersona.esercizi.Add(Console.ReadLine());

                Console.WriteLine($"Carico per l'esercizio {j + 1}:");
                nuovaPersona.carichi.Add(Convert.ToInt32(Console.ReadLine()));

                Console.WriteLine("Quale è la data di utilizzo del carico?(yyyy-MM-dd)");
                nuovaPersona.data = Convert.ToDateTime(Console.ReadLine());
            }

            persone[n] = nuovaPersona;
            n++;
        }
    }

    static void salvaPersone(Persona[] persone, int n, string nomeFile)
    {
        using StreamWriter file = new StreamWriter(nomeFile);
        for (int i = 0; i < n; i++)
        {
            string eserciziStr = string.Join(",", persone[i].esercizi);
            string carichiStr = string.Join(",", persone[i].carichi.Select(c => c.ToString("F2")));
            string dataStr = persone[i].data.ToString("yyyy-MM-dd");

            string linea = $"{persone[i].nome};{eserciziStr};{carichiStr};{dataStr}";
            file.WriteLine(linea);
        }
    }

    static void modificaPersone(Persona[] persone, int n, string nomeFile)
    {
        Console.WriteLine("Inserisci il nome della persona da modificare:");
        string nomeDaCercare = Console.ReadLine();
        bool trovata = false;

        for (int i = 0; i < n; i++)
        {
            if (persone[i].nome.Equals(nomeDaCercare, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Inserisci il nuovo nome (lascia vuoto per non cambiarlo):");
                string nuovoNome = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(nuovoNome))
                    persone[i].nome = nuovoNome;

                Console.WriteLine("Inserisci un nuovo esercizio da aggiungere:");
                string nuovoEsercizio = Console.ReadLine();
                persone[i].esercizi.Add(nuovoEsercizio);

                Console.WriteLine("Inserisci il nuovo carico:");
                int nuovoCarico = Convert.ToInt32(Console.ReadLine());
                persone[i].carichi.Add(nuovoCarico);

                Console.WriteLine("Inserisci la nuova data:");
                persone[i].data = Convert.ToDateTime(Console.ReadLine());

                int ultimoIndiceUguale = -1;
                for (int j = persone[i].esercizi.Count - 2; j >= 0; j--)
                {
                    if (persone[i].esercizi[j].Equals(nuovoEsercizio, StringComparison.OrdinalIgnoreCase))
                    {
                        ultimoIndiceUguale = j;
                        break;
                    }
                }

                if (ultimoIndiceUguale != -1)
                {
                    double caricoPrecedente = persone[i].carichi[ultimoIndiceUguale];
                    double miglioramento = MiglioramentoPerc(nuovoCarico, caricoPrecedente);
                    Console.WriteLine($"{persone[i].nome} ha migliorato il carico di {miglioramento:F2}% sull'esercizio {nuovoEsercizio} rispetto al carico precedente di {caricoPrecedente} kg.");
                }
                else
                {
                    Console.WriteLine($"Nessun carico precedente registrato per l'esercizio {nuovoEsercizio}, quindi non è possibile calcolare un miglioramento.");
                }

                trovata = true;
                break;
            }
        }

        if (trovata)
        {
            salvaPersone(persone, n, nomeFile);
            Console.WriteLine("Persona modificata con successo!");
        }
        else
        {
            Console.WriteLine("Persona non trovata.");
        }
    }

    static void eliminaPersona(ref Persona[] persone, ref int n, string nomeFile)
    {
        Console.WriteLine("Inserisci il nome della persona da eliminare:");
        string nomeDaCercare = Console.ReadLine();
        bool trovata = false;

        for (int i = 0; i < n; i++)
        {
            if (persone[i].nome.ToLower() == nomeDaCercare.ToLower())
            {
                Console.WriteLine($"Sei sicuro di voler eliminare {persone[i].nome}? (s/n)");
                string conferma = Console.ReadLine();

                if (conferma.ToLower() == "s")
                {
                    for (int j = i; j < n - 1; j++)
                    {
                        persone[j] = persone[j + 1];
                    }
                    n--;
                    salvaPersone(persone, n, nomeFile);
                    Console.WriteLine("Persona eliminata.");
                }
                else
                {
                    Console.WriteLine("Eliminazione annullata.");
                }

                trovata = true;
                break;
            }
        }

        if (!trovata)
        {
            Console.WriteLine("Persona non trovata.");
        }
    }

    static void stampaPersone(Persona[] persone, int n)
    {
        if (n == 0)
        {
            Console.WriteLine("Nessuna persona da mostrare.");
            return;
        }

        for (int i = 0; i < n; i++)
        {
            Console.WriteLine($"\n[{i + 1}] {persone[i].nome} - Data ultima modifica: {persone[i].data:yyyy-MM-dd}");

            if (persone[i].esercizi.Count == 0 || persone[i].carichi.Count == 0)
            {
                Console.WriteLine("  Nessun esercizio/carico registrato.");
                continue;
            }

            for (int j = 0; j < persone[i].esercizi.Count && j < persone[i].carichi.Count; j++)
            {
                Console.WriteLine($"  - Esercizio: {persone[i].esercizi[j]}, Carico: {persone[i].carichi[j]} kg");
            }
        }
    }

    static int personeCaricate(Persona[] persone, string nomeFile)
    {
        int conta = 0;

        if (!File.Exists(nomeFile))
            return 0;

        using (StreamReader file = new StreamReader(nomeFile))
        {
            string riga;
            while ((riga = file.ReadLine()) != null)
            {
                string[] dati = riga.Split(';');
                if (dati.Length < 4) continue;

                Persona p = new Persona
                {
                    nome = dati[0],
                    esercizi = new List<string>(dati[1].Split(',')),
                    carichi = new List<int>(),
                    data = DateTime.Parse(dati[3])
                };

                foreach (var val in dati[2].Split(','))
                {
                    if (int.TryParse(val, out int carico))
                        p.carichi.Add(carico);
                }

                persone[conta] = p;
                conta++;
            }
        }

        return conta;
    }

    static void mostraProgressioneEsercizio(Persona[] persone, int n)
    {
        Console.WriteLine("Inserisci il nome della persona:");
        string nomePersona = Console.ReadLine();

        Persona personaTrovata = default;
        bool trovata = false;

        for (int i = 0; i < n; i++)
        {
            if (persone[i].nome.Equals(nomePersona, StringComparison.OrdinalIgnoreCase))
            {
                personaTrovata = persone[i];
                trovata = true;
                break;
            }
        }

        if (!trovata)
        {
            Console.WriteLine("Persona non trovata.");
            return;
        }

        Console.WriteLine("Inserisci il nome dell'esercizio:");
        string nomeEsercizio = Console.ReadLine();

        List<(DateTime data, double carico)> progressione = new List<(DateTime, double)>();

        for (int i = 0; i < personaTrovata.esercizi.Count; i++)
        {
            if (personaTrovata.esercizi[i].Equals(nomeEsercizio, StringComparison.OrdinalIgnoreCase))
            {
                progressione.Add((personaTrovata.data, personaTrovata.carichi[i]));
            }
        }

        if (progressione.Count == 0)
        {
            Console.WriteLine("Nessun dato trovato per questo esercizio.");
            return;
        }

        Console.WriteLine($"\nProgressione dei carichi per {nomeEsercizio} di {personaTrovata.nome}:");
        foreach (var entry in progressione)
        {
            Console.WriteLine($"Data: {entry.data:yyyy-MM-dd}, Carico: {entry.carico} kg");
        }
    }

    private static void Main(string[] args)
    {
        Persona[] persone = new Persona[100];
        int n = 0;
        bool modificato = false;
        char scelta;
        string fileDati = "dati_personali.txt";

        n = personeCaricate(persone, fileDati);

        do
        {
            Console.WriteLine("MENU:");
            Console.WriteLine($"Persone presenti: {n}");
            Console.WriteLine("1 - Inserisci persone");
            Console.WriteLine("2 - Salva persone su file");
            Console.WriteLine("3 - Modifica persona (con aumento carico %)");
            Console.WriteLine("4 - Elimina persona");
            Console.WriteLine("5 - Elenco persone (con dati)");
            Console.WriteLine("6 - Mostra progressione carichi per esercizio");
            Console.WriteLine("0 - Esci");
            Console.Write("Scelta: ");

            scelta = Convert.ToChar(Console.ReadLine());
            Console.WriteLine();

            switch (scelta)
            {
                case '1':
                    inserisciPersone(persone, ref n);
                    modificato = true;
                    break;
                case '2':
                    salvaPersone(persone, n, fileDati);
                    Console.WriteLine("Persone salvate su file.");
                    modificato = false;
                    break;
                case '3':
                    modificaPersone(persone, n, fileDati);
                    modificato = true;
                    break;
                case '4':
                    eliminaPersona(ref persone, ref n, fileDati);
                    break;
                case '5':
                    stampaPersone(persone, n);
                    break;
                case '6':
                    mostraProgressioneEsercizio(persone, n);
                    break;
                case '0':
                    Console.WriteLine("Uscita dal programma.");
                    break;
                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }

            Console.WriteLine("\nPremi INVIO per continuare...");
            Console.ReadLine();

        } while (scelta != '0');

        if (modificato)
        {
            Console.WriteLine("Salvare? (s/n)");
            char risp = Convert.ToChar(Console.ReadLine());
            if (risp == 's')
            {
                salvaPersone(persone, n, fileDati);
                Console.WriteLine("Persone salvate su file.");
            }
            else
            {
                Console.WriteLine("Modifiche non salvate.");
            }
        }
    }
}
