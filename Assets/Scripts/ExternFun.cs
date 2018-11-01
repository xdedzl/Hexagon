using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExternFun {

	public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : direction - 3;
    }
}
