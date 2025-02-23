using ConnectorDataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WConnectorModels
{
    public class FavoritoModel
    {
        public string MpCode { get; set; }
        public string MpDesc { get; set; }
    }

    public class Favorite
    {
        public List<FavoritoModel> Favoritos { get; set; }

        public List<NEWAGE> ErpData { get; set; }

        public void AddFavorite(FavoritoModel favorito)
        {
            if (!Favoritos.Contains(favorito))
            {
                Favoritos.Add(favorito);


            }
        }

        public void DeleteFavorite(string Code)
        {

        }

        public void LoadFavorites()
        {

        }

    }
}
