using System;

namespace Kutuphane1.Helpers
{
    public record Filtre
    {
        public string[] KitapAdiId { get; set; } //**
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
    }
}