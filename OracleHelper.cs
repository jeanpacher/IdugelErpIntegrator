using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using System.Security.Cryptography;
using System.Data;
using Inventor;

namespace IdugelErpIntegrator
{
    public static class OracleHelper
    {
        public static OracleConnection connection = null;
        public static bool GetConnection()
        {
            try
            {
                string connectionString = "User Id=idugel_v9;Password=4725;" +
                              "Data Source=(DESCRIPTION=(ADDRESS_LIST=" +
                              "(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.50.205)(PORT=1521)))" +
                              "(CONNECT_DATA=(SID=orcl)));";
                connection = new OracleConnection(connectionString);
                connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao conectar no banco de dados: " + ex.Message);
                MessageBox.Show("Erro ao conectar no banco de dados: " + ex.Message);
                return false;
            }
        }
        public static DataTable PesquisarMateriaPrimaPorCodigo(string codigo)
        {
            DataTable dt = new DataTable();
            try
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = OracleHelper.connection; // Objeto OracleConnection já existente

                    cmd.CommandText = @"SELECT 
                                            g.CODIGO, 
                                            g.DESCR, 
                                            g.FAMILIA AS FAMILIA_CODIGO, 
                                            f.DESCR AS FAMILIA_NOME, 
                                            g.FAMILIA || ' - ' || f.DESCR AS FAMILIA_COMPLETA,
                                            g.UNIDADE,
                                            gi.DESCINV,
                                            gi.ESPMP,
                                            gi.FATORUN
                                        FROM 
                                            Geral g
                                        LEFT JOIN 
                                            Familia f ON g.FAMILIA = f.CODIGO
                                        LEFT JOIN 
                                            GERAL_INVENTOR gi ON g.CODIGO = gi.CODIGO
                                        WHERE 
                                            g.CODIGO LIKE :codigo || '%'";


                    //cmd.CommandText = @"SELECT 
                    //                        g.CODIGO, 
                    //                        g.DESCR, 
                    //                        g.FAMILIA AS FAMILIA_CODIGO, 
                    //                        f.DESCR AS FAMILIA_NOME, 
                    //                        g.FAMILIA || ' - ' || f.DESCR AS FAMILIA_COMPLETA,
                    //                        g.UNIDADE
                    //                    FROM 
                    //                        Geral g
                    //                    LEFT JOIN 
                    //                        Familia f ON g.FAMILIA = f.CODIGO
                    //                    WHERE 
                    //                        g.CODIGO LIKE :codigo || '%'";




                    //cmd.CommandText = "SELECT CODIGO, DESCR, FAMILIA, UNIDADE FROM Geral WHERE CODIGO LIKE :codigo || '%'";

                    cmd.Parameters.Add(new OracleParameter("codigo", codigo));

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {

                        da.Fill(dt);
                    }



                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao pesquisar materiais: {ex.Message}");
            }

            return dt;
        }
        public static DataTable PesquisarMateriaPrimaPorDescricao(string desc)
        {
            DataTable dt = new DataTable();
            try
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = OracleHelper.connection; // Objeto OracleConnection já existente

                    cmd.CommandText = @"SELECT 
                                            g.CODIGO, 
                                            g.DESCR, 
                                            g.FAMILIA AS FAMILIA_CODIGO, 
                                            f.DESCR AS FAMILIA_NOME, 
                                            g.FAMILIA || ' - ' || f.DESCR AS FAMILIA_COMPLETA,
                                            g.UNIDADE,
                                            gi.DESCINV,
                                            gi.ESPMP,
                                            gi.FATORUN
                                        FROM 
                                            Geral g
                                        LEFT JOIN 
                                            Familia f ON g.FAMILIA = f.CODIGO
                                        LEFT JOIN 
                                            GERAL_INVENTOR gi ON g.CODIGO = gi.CODIGO
                                        WHERE 
                                            g.DESCR LIKE :descricao || '%'";


                    //cmd.CommandText = @"SELECT 
                    //                        g.CODIGO, 
                    //                        g.DESCR, 
                    //                        g.FAMILIA AS FAMILIA_CODIGO, 
                    //                        f.DESCR AS FAMILIA_NOME, 
                    //                        g.FAMILIA || ' - ' || f.DESCR AS FAMILIA_COMPLETA,
                    //                        g.UNIDADE
                    //                    FROM 
                    //                        Geral g
                    //                    LEFT JOIN 
                    //                        Familia f ON g.FAMILIA = f.CODIGO
                    //                    WHERE 
                    //                        g.DESCR LIKE :descricao || '%'";

                    //cmd.CommandText = "SELECT CODIGO, DESCR, FAMILIA, UNIDADE FROM Geral WHERE CODIGO LIKE :codigo || '%'";

                    cmd.Parameters.Add(new OracleParameter("descricao", desc));

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {

                        da.Fill(dt);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao pesquisar materiais: {ex.Message}");
            }

            return dt;
        }
        public static string GetDescricaoByCodigo(string codigo)
        {

            string descricao = null;
            string query = $"SELECT * FROM GERAL WHERE CODIGO = '{codigo}'";

            try
            {
                // Cria o comando Oracle
                using (OracleCommand cmd = new OracleCommand(query, connection))
                {
                    // Define o parâmetro da consulta
                    // cmd.Parameters.Add(new OracleParameter("codigo", codigo));

                    // Executa o comando e lê o resultado
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            descricao = reader["DESCR"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao buscar a descrição: " + ex.Message);
            }

            return descricao;
        }
        public static void UpdateMateriaPrimaOnNewAge(string codigo, string descricaoInventor, double espessura, double fatorUnidade)
        {
            
            try
            {

                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = OracleHelper.connection;
                    cmd.CommandText = @"
                                        MERGE INTO GERAL_INVENTOR gi
                                        USING (SELECT :codigo AS CODIGO FROM DUAL) src
                                        ON (gi.CODIGO = src.CODIGO)
                                        WHEN MATCHED THEN
                                            UPDATE SET DESCINV = :descricaoInventor, ESPMP = :espessura, FATORUN = :fatorUnidade
                                        WHEN NOT MATCHED THEN
                                            INSERT (CODIGO, DESCINV, ESPMP, FATORUN) 
                                            VALUES (:codigo, :descricaoInventor, :espessura, :fatorUnidade)";

                    cmd.Parameters.Add(new OracleParameter("codigo", codigo));
                    cmd.Parameters.Add(new OracleParameter("descricaoInventor", descricaoInventor));
                    cmd.Parameters.Add(new OracleParameter("espessura", espessura));
                    cmd.Parameters.Add(new OracleParameter("fatorUnidade", fatorUnidade));

                    cmd.ExecuteNonQuery();
                }
                Logger.Log($"Matéria-prima atualizada: Código={codigo}, Espessura={espessura}, Fator Unidade={fatorUnidade}");
                MessageBox.Show("Informações salvas com sucesso!");
            }
            catch (OracleException ex)
            {
                Logger.Log($"Erro ao atualizar matéria-prima: Não foi possivel acessar o banco de dados:{ex.Message}");
                MessageBox.Show($"Erro ao acessar o banco de dados: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.Log($"Erro Inexperado ao atualizar matéria-prima: {ex.Message}");
                MessageBox.Show($"Erro inesperado: {ex.Message}");
            }

        }
        public static List<string> GetUnidades()
        {
            List<string> unidades = new List<string>();
            string query = "SELECT CODIGO FROM UNIDADES";

            try
            {
                // Cria o comando Oracle
                using (OracleCommand cmd = new OracleCommand(query, connection))
                {
                    // Executa o comando
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Adiciona a descrição de cada unidade à lista
                            unidades.Add(reader["CODIGO"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao buscar unidades: " + ex.Message);
            }

            return unidades;
        }
    }
}
