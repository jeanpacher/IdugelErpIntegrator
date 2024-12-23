using System;
using System.Collections.Generic;
using System.Linq;

namespace PaintManager
{

    public class PaintGroupValues
    {
        private List<double> Padrao;
        private List<double> Interno;
        private List<double> Laranja;
        private List<double> Amarelo;
        private List<double> Outros;

        public PaintGroupValues()
        {
            Padrao = new List<double>();
            Interno = new List<double>();
            Laranja = new List<double>();
            Amarelo = new List<double>();
            Outros = new List<double>();
        }

        private void AddValue(double val, List<double> list)
        {
            list.Add(val);
        }

        private double GetSumValue(List<double> list)
        {
            return Math.Round(list.Sum(), 6);
        }

        public void AddPadrao(double val)
        {
            AddValue(val, Padrao);
        }

        public double GetTotalPadrao()
        {
            return GetSumValue(Padrao);
        }

        public void AddInterno(double val)
        {
            AddValue(val, Interno);
        }

        public double GetTotalInterno()
        {
            return GetSumValue(Interno);
        }

        public void AddLaranja(double val)
        {
            AddValue(val, Laranja);
        }

        public double GetTotalLaranja()
        {
            return GetSumValue(Laranja);
        }

        public void AddAmarelo(double val)
        {
            AddValue(val, Amarelo);
        }

        public double GetTotalAmarelo()
        {
            return GetSumValue(Amarelo);
        }

        public void AddOutros(double val)
        {
            AddValue(val, Padrao);
        }

        public double GetTotalOutros()
        {
            return GetSumValue(Outros);
        }

    }

}