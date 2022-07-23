using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardingHouse.Models
{
    public class RoomModelView : Room
    {
        public IPagedList<Room> PageList;
    }
}