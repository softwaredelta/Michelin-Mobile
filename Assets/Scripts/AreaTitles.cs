using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AreaTitles : MonoBehaviour
{
    public static Dictionary<int, string> LoadAreaTitles(int scene)
    {
        var TitleDictionary = new Dictionary<int, string>();
        if (scene == 3)
        {
            TitleDictionary[1] = "Michelin Lifestyle";
            TitleDictionary[2] = "Área de Exhibición";
            TitleDictionary[3] = "Exhibidores";
            TitleDictionary[4] = "Materiales de Trabajo";
            TitleDictionary[5] = "Servicio de Refriguerio";
            TitleDictionary[6] = "Sala de Espera";
            TitleDictionary[7] = "Baños";
            TitleDictionary[8] = "Internet";
            TitleDictionary[9] = "Ambientación";
            TitleDictionary[10] = "Área de Juegos";
            TitleDictionary[11] = "Inmobiliario";
        }
        else if (scene == 2)
        {
            TitleDictionary[1] = "Estacionamiento";
            TitleDictionary[2] = "Tienda";
            TitleDictionary[3] = "Facahada y Faldón";
            TitleDictionary[4] = "Bandera Trimarca";
            TitleDictionary[5] = "Iluminación";
            TitleDictionary[6] = "Residuos Peligrosos";
            TitleDictionary[7] = "Paredes";
            TitleDictionary[8] = "Música";
            TitleDictionary[9] = "Patio de Servicio";
            TitleDictionary[10] = "Rampas";
            TitleDictionary[11] = "Taller";
            TitleDictionary[12] = "Acceso para personas con capacidades diferentes";
            TitleDictionary[13] = "Menú de Servicios";
        }
        else if (scene == 1)
        {
            TitleDictionary[1] = "Preparación";
        }
        else if (scene == 4)
        {
            TitleDictionary[1] = "Cliente";
        }
        else if (scene == 5)
        {
            TitleDictionary[1] = "Gerente";
        }

        return TitleDictionary;
    }
}