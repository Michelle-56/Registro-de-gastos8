using System;
using System.Collections.Generic;
using System.IO;

namespace Registro_Gastos
{
    public class Gasto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public string Categoria { get; set; }

        public Gasto(int id, string descripcion, decimal monto, string categoria)
        {
            Id = id;
            Descripcion = descripcion;
            Monto = monto;
            Categoria = categoria;
        }

        public override string ToString()
        {
            return $"{Id} {Descripcion} Q {Monto:F2} {Categoria}";
        }
    }

    internal class Program
    {
        private static List<Gasto> gastos = new List<Gasto>();
        private static string archivo = "gastos.csv";

        public static void Main(string[] args)
        {
            CargarCSV();

            int siguienteId = ObtenerSiguienteId();
            int opcion;

            do
            {
                Console.WriteLine();
                Console.WriteLine("=================================");
                Console.WriteLine("       REGISTRO DE GASTOS");
                Console.WriteLine("=================================");
                Console.WriteLine("1. Agregar gasto");
                Console.WriteLine("2. Listar gastos");
                Console.WriteLine("3. Buscar por categoría");
                Console.WriteLine("4. Salir");
                Console.WriteLine("=================================");
                Console.Write("Elige una opción: ");

                string entrada = Console.ReadLine();

                if (!int.TryParse(entrada, out opcion))
                {
                    opcion = 0;
                }

                Console.WriteLine();

                switch (opcion)
                {
                    case 1:
                        AgregarGasto(ref siguienteId);
                        break;

                    case 2:
                        ListarGastos();
                        break;

                    case 3:
                        BuscarPorCategoria();
                        break;

                    case 4:
                        GuardarCSV();
                        Console.WriteLine("Saliendo...");
                        break;

                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }

            } while (opcion != 4);
        }

        private static void AgregarGasto(ref int siguienteId)
        {
            Console.Write("Descripción: ");
            string descripcion = Console.ReadLine();

            Console.Write("Monto: ");
            string entradaMonto = Console.ReadLine();

            Console.Write("Categoría: ");
            string categoria = Console.ReadLine();

            decimal monto;

            bool montoValido = decimal.TryParse(
                entradaMonto,
                out monto
            );

            if (string.IsNullOrWhiteSpace(descripcion) ||
                !montoValido ||
                monto <= 0 ||
                string.IsNullOrWhiteSpace(categoria))
            {
                Console.WriteLine("Datos inválidos.");
                return;
            }

            Gasto nuevoGasto = new Gasto(
                siguienteId,
                descripcion,
                monto,
                categoria
            );

            gastos.Add(nuevoGasto);

            siguienteId++;

            Console.WriteLine("Gasto agregado.");
        }

        private static void ListarGastos()
        {
            if (gastos.Count == 0)
            {
                Console.WriteLine("No hay gastos registrados.");
                return;
            }

            foreach (Gasto gasto in gastos)
            {
                Console.WriteLine(
                    $"{gasto.Id} {gasto.Descripcion} Q {gasto.Monto:F2} {gasto.Categoria}"
                );
            }

            decimal total = 0;

            foreach (Gasto gasto in gastos)
            {
                total += gasto.Monto;
            }

            Console.WriteLine();
            Console.WriteLine($"TOTAL GASTADO: Q {total:F2}");
        }

        private static void BuscarPorCategoria()
        {
            Console.Write("Categoría a buscar: ");
            string categoriaBuscada = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(categoriaBuscada))
            {
                Console.WriteLine("La categoría no puede estar vacía.");
                return;
            }

            bool encontrado = false;

            foreach (Gasto gasto in gastos)
            {
                if (gasto.Categoria.Equals(
                    categoriaBuscada,
                    StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine(
                        $"{gasto.Id} {gasto.Descripcion} Q {gasto.Monto:F2} {gasto.Categoria}"
                    );

                    encontrado = true;
                }
            }

            if (!encontrado)
            {
                Console.WriteLine(
                    "No se encontraron gastos en esa categoría."
                );
            }
        }

        private static void GuardarCSV()
        {
            using (StreamWriter escritor = new StreamWriter(archivo))
            {
                escritor.WriteLine("Id;Descripcion;Monto;Categoria");

                foreach (Gasto gasto in gastos)
                {
                    escritor.WriteLine(
                        $"{gasto.Id};{gasto.Descripcion};{gasto.Monto:F2};{gasto.Categoria}"
                    );
                }
            }

            Console.WriteLine(
                $"Gastos guardados en {archivo} ({gastos.Count} registros)."
            );
        }

        private static void CargarCSV()
        {
            if (!File.Exists(archivo))
            {
                return;
            }

            string[] lineas = File.ReadAllLines(archivo);

            for (int i = 1; i < lineas.Length; i++)
            {
                string[] datos = lineas[i].Split(';');

                if (datos.Length == 4)
                {
                    int id;
                    decimal monto;

                    bool idValido = int.TryParse(
                        datos[0],
                        out id
                    );

                    bool montoValido = decimal.TryParse(
                        datos[2],
                        out monto
                    );

                    if (idValido && montoValido)
                    {
                        Gasto gasto = new Gasto(
                            id,
                            datos[1],
                            monto,
                            datos[3]
                        );

                        gastos.Add(gasto);
                    }
                }
            }

            Console.WriteLine(
                $"Cargados {gastos.Count} gastos desde {archivo}."
            );
        }

        private static int ObtenerSiguienteId()
        {
            if (gastos.Count == 0)
            {
                return 1;
            }

            int mayorId = 0;

            foreach (Gasto gasto in gastos)
            {
                if (gasto.Id > mayorId)
                {
                    mayorId = gasto.Id;
                }
            }

            return mayorId + 1;
        }
    }
}

