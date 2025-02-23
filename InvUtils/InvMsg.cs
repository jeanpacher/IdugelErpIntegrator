using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;

namespace InvUtils
{
    public static class InvMsg
    {
        public static void Msg(string mensagem)
        {
            ErrorManager errorManager = InvApp.StandardAddInServer.m_InvApp.ErrorManager;

            errorManager.AddMessage(mensagem, false);

            errorManager.Show("Aviso:", true, false);
        }
    }
}