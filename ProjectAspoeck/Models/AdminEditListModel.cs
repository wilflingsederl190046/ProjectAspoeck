﻿namespace ProjectAspoeck.Models;

public class AdminEditListModel
{
    public string SessionKey { get; set; }
    public List<ItemDto> Items   { get; set; }
    
    public List<ImageDto> Images { get; set; }
}