using UnityEngine;

namespace WildIsland.Extras
{
    public static class Extensions
    {
        public static bool Contains(this RectTransform rect1, RectTransform rect2)
        {
            int newX = (int)rect1.sizeDelta.x / 2;
            int newY = (int)rect1.sizeDelta.y / 2;

            Vector2 newUpperLeftVertex = new Vector2(-newX + (int)rect1.position.x, newY + (int)rect1.position.y);
            Vector2 newUpperRightVertex = new Vector2(newX + (int)rect1.position.x, newY + (int)rect1.position.y);
            Vector2 newLowerLeftVertex = new Vector2(-newX + (int)rect1.position.x, -newY + (int)rect1.position.y);
            Vector2 newLowerRightVertex = new Vector2(newX + (int)rect1.position.x, -newY + (int)rect1.position.y);

            int x = (int)rect2.sizeDelta.x / 2;
            int y = (int)rect2.sizeDelta.y / 2;

            Vector2 upperLeftVertex = new Vector2(-x + (int)rect2.position.x, y + (int)rect2.position.y);
            Vector2 upperRightVertex = new Vector2(x + (int)rect2.position.x, y + (int)rect2.position.y);
            Vector2 lowerLeftVertex = new Vector2(-x + (int)rect2.position.x, -y + (int)rect2.position.y);
            Vector2 lowerRightVertex = new Vector2(x + (int)rect2.position.x, -y + (int)rect2.position.y);

            if (newUpperLeftVertex.x <= lowerRightVertex.x &&
                newUpperLeftVertex.x >= lowerLeftVertex.x &&
                newUpperLeftVertex.y >= lowerRightVertex.y &&
                newUpperLeftVertex.y <= upperRightVertex.y)
                return true;

            if (newUpperRightVertex.x >= lowerLeftVertex.x &&
                newUpperRightVertex.x <= lowerRightVertex.x &&
                newUpperRightVertex.y >= lowerLeftVertex.y &&
                newUpperRightVertex.y <= upperLeftVertex.y)
                return true;

            if (newLowerLeftVertex.x <= upperRightVertex.x &&
                newLowerLeftVertex.x >= upperLeftVertex.x &&
                newLowerLeftVertex.y <= upperRightVertex.y &&
                newLowerLeftVertex.y >= lowerRightVertex.y)
                return true;

            return newLowerRightVertex.x >= upperLeftVertex.x &&
                   newLowerRightVertex.x <= upperRightVertex.x &&
                   newLowerRightVertex.y <= upperLeftVertex.y &&
                   newLowerRightVertex.y >= lowerLeftVertex.y;
        }
    }
}