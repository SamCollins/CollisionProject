using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Collision_Simulator
{
    #region Support Classes
    class Rectangle
    {
        public float x;
        public float y;
        public float w;
        public float h;

        public Rectangle(float width, float height)
        {
            x = width / 2;
            y = height / 2;
            w = width / 2;
            h = height / 2;
        }

        public Rectangle(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            w = width;
            h = height;
        }

        public static bool BoxCheck(Component A, Component B, float diffX, float diffY)
        {
            BoundingBox BoxA = new BoundingBox(A, 0, 0);
            BoundingBox BoxB = new BoundingBox(B, diffX + B.xOff - A.xOff, diffY + B.yOff - A.yOff);

            float d1x = BoxB.minX - BoxA.maxX;
            float d1y = BoxB.minY - BoxA.maxY;
            float d2x = BoxA.minX - BoxB.maxX;
            float d2y = BoxA.minY - BoxB.maxY;

            if (d1x > 0.0f || d1y > 0.0f)
                return false;

            if (d2x > 0.0f || d2y > 0.0f)
                return false;

            return true;
        }

        public bool Contains(Entity entity)
        {
            Component rect = new Component();
            rect.size = w;
            rect.type = 2;
            float diffX = x - entity.x;
            float diffY = y - entity.y;

            for (int i = 0; i < entity.c.Length; i++)
            {
                if (BoxCheck(entity.c[i], rect, diffX, diffY))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Intersects(Rectangle range)
        {
            Component A = new Component();
            A.size = this.w;
            A.type = 2;
            Component B = new Component();
            B.size = range.w;
            B.type = 2;

            float diffX = range.x - this.x;
            float diffY = range.y - this.y;

            return BoxCheck(A, B, diffX, diffY);
        }

    }

    class QuadTree
    {
        public Rectangle boundary;
        public int capacity;
        public Dictionary<Entity, int> entities;

        public QuadTree northwest;
        public QuadTree northeast;
        public QuadTree southwest;
        public QuadTree southeast;

        public bool divided;

        public QuadTree(int width, int height, int capacity)
        {
            boundary = new Rectangle(width, height);
            this.capacity = capacity;
            entities = new Dictionary<Entity, int>();
            divided = false;
        }

        private QuadTree(Rectangle boundary, int capacity)
        {
            this.boundary = boundary;
            this.capacity = capacity;
            entities = new Dictionary<Entity, int>();
            divided = false;
        }

        public void Insert(Entity entity, int id)
        {
            if (!boundary.Contains(entity))
            {
                return;
            }

            if (entities.Count < capacity)
            {
                entities.Add(entity, id);
            }
            else
            {
                if (!divided)
                {
                    SubDivide();
                }
                northeast.Insert(entity, id);
                northwest.Insert(entity, id);
                southeast.Insert(entity, id);
                southwest.Insert(entity, id);
            }
        }

        public void SubDivide()
        {
            float x = boundary.x;
            float y = boundary.y;
            float w = boundary.w;
            float h = boundary.h;

            Rectangle ne = new Rectangle(x + w / 2, y - h / 2, w / 2, h / 2);
            northeast = new QuadTree(ne, capacity);
            Rectangle nw = new Rectangle(x - w / 2, y - h / 2, w / 2, h / 2);
            northwest = new QuadTree(nw, capacity);
            Rectangle se = new Rectangle(x + w / 2, y + h / 2, w / 2, h / 2);
            southeast = new QuadTree(se, capacity);
            Rectangle sw = new Rectangle(x - w / 2, y + h / 2, w / 2, h / 2);
            southwest = new QuadTree(sw, capacity);

            divided = true;
        }

        public void Query(Rectangle range, HashSet<int> found)
        {
            if (!boundary.Intersects(range))
            {
                return;
            }
            else
            {
                foreach (Entity ent in entities.Keys)
                {
                    if (range.Contains(ent))
                    {
                        found.Add(entities[ent]);
                    }
                }
                if (divided)
                {
                    northeast.Query(range, found);
                    northwest.Query(range, found);
                    southeast.Query(range, found);
                    southwest.Query(range, found);
                }
            }
        }

        public int[] GetPossibleCollisions(Entity search)
        {
            HashSet<int> found = new HashSet<int>();

            for (int i = 0; i < search.c.Length; i++)
            {
                Query(new Rectangle(search.x + search.c[i].xOff, search.y + search.c[i].yOff,
                    search.c[i].size, search.c[i].size), found);
            }

            return found.ToArray();
        }
    }
    #endregion
    #region Structs
    public struct Component
    {
        public int type; //0 = circle, 2 = square
        public float size;
        public float xOff;
        public float yOff;
    }
    public struct Entity
    {
        public Component[] c;
        public float x;
        public float y;
        public float x_speed;
        public float y_speed;
        public Color color;
        public bool colliding;
    }
    public struct BoundingBox
    {
        public float minX;
        public float minY;
        public float maxX;
        public float maxY;

        public BoundingBox(Component comp, float posX, float posY)
        {
            minX = posX - comp.size;
            minY = posY - comp.size;
            maxX = posX + comp.size;
            maxY = posY + comp.size;
        }
    }
    public struct Cell
    {
        public int[] eRefs;
    }
    public struct Grid
    {
        public int CellSize;
        public Cell[,] Cells;
    }
    #endregion

    public partial class CollisonSimulator : Form
    {

        #region Globals
        //Misc
        public long lastDrawn = 0;
        public float speedMod = 1;
        public Random rand;

        //Entity Creation
        public int componentType = 2;
        public bool bLaunchingEntity = false;
        public bool bCreatingEntity = false;
        public bool bCreatingComponent = false;
        public Entity newEntity;
        public Component newComponent;
        public Entity[] entities;

        public Grid grid;

        //Pixel Perfect Checking
        public float vCanvasScale = 1.0f;
        public int[,] vCanvas;

        //Strategies
        public int collisionMethod = 1; //0 = bounds checking, 1 = pixel perfect
        public int collisionStrategy = 0; //0 = Bruce Force, 1 = QuadTree, 2 = Spatial Hashing
        
        //Threading
        Mutex gfxMutex = new Mutex(false, "gfxMutex");
        Thread moveThread;
        #endregion

        #region Startup/Shutdown
        public CollisonSimulator()
        {
            //Setup Window
            InitializeComponent();
            CenterToScreen();
            DoubleBuffered = true;

            //Setup logic
            rand = new Random();
            entities = new Entity[0];

            moveThread = new Thread(MovementThread);
            moveThread.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            moveThread.Abort();
        }
        #endregion

        #region Drawing
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            drawEntities(e.Graphics);
        }

        public void drawEntities(Graphics gfx)
        {
            gfxMutex.WaitOne();
            foreach (Entity e in entities)
            {
                drawEntity(gfx, e);
            }
            gfxMutex.ReleaseMutex();
            if (bCreatingEntity || bLaunchingEntity)
            {
                drawEntity(gfx, newEntity);
            }
            if (bLaunchingEntity)
            {
                gfx.DrawLine(new Pen(Color.DarkRed), newEntity.x, newEntity.y, newEntity.x + newEntity.x_speed, newEntity.y + newEntity.y_speed);
            }
        }

        public void drawEntity(Graphics gfx, Entity e)
        {
            Brush br;
            if (e.colliding)
            {
                br = new SolidBrush(Color.Red);
            }
            else
            {
                br = new SolidBrush(e.color);
            }

            foreach (Component c in e.c)
            {
                switch (c.type)
                {
                    case 0:
                        //Circle
                        if (c.size == 0) { break; }
                        gfx.FillEllipse(br, e.x + c.xOff - c.size, e.y + c.yOff - c.size, c.size * 2, c.size * 2);
                        break;
                    case 1:
                        //Reserved for Future Use
                        if (c.size == 0) { break; }
                        break;
                    case 2:
                        //Square
                        if (c.size == 0) { break; }
                        gfx.FillRectangle(br, e.x + c.xOff - c.size, e.y + c.yOff - c.size, c.size * 2, c.size * 2);
                        break;
                }
            }
        }

        public void invalidateEntity(Entity e)
        {
            foreach (Component c in e.c)
            {
                invalidateComponent(c, (int)e.x, (int)e.y);
            }
        }

        public void invalidateComponent(Component c, int x, int y)
        {
            int buffer = (int)c.size + 10;

            Invalidate(new System.Drawing.Rectangle((int)(x + c.xOff - buffer), (int)(y + c.yOff - buffer), buffer * 2, buffer * 2));
        }
        #endregion

        #region Creating Entities/Components/Motion
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                //Left Mouse Button
                if (!bCreatingEntity)
                {
                    startEntity(e.X, e.Y);
                }

                if (bCreatingEntity && !bCreatingComponent)
                {
                    startComponent(e.X, e.Y);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                //Right Mouse Button
                if (bCreatingEntity && !bCreatingComponent)
                {
                    startMotion(e.X, e.Y);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (bCreatingComponent)
            {
                updateComponent(e.X, e.Y);
                invalidateEntity(newEntity);
            }

            if (bLaunchingEntity)
            {
                updateMotion(e.X, e.Y);
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                //Left Mouse Button
                if (bCreatingComponent)
                {
                    endComponent(e.X, e.Y);
                    Invalidate();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                //Right Mouse Button
                if (bLaunchingEntity)
                {
                    endMotion(e.X, e.Y);
                    Invalidate();
                }
            }
        }

        public void startEntity(int x, int y)
        {
            //Create a new Entity(Component will be added after inside MouseDown)
            bCreatingEntity = true;
            newEntity = new Entity();
            newEntity.c = new Component[0];
            newEntity.x = x;
            newEntity.y = y;
            newEntity.x_speed = 0;
            newEntity.y_speed = 0;
            newEntity.color = Color.FromArgb(0, (int)Math.Pow(rand.Next() % 11, 2) * (rand.Next(1) - 1) + 128, (int)Math.Pow(rand.Next() % 11, 2) * (rand.Next(1) - 1) + 128);
            newEntity.colliding = false;
        }

        public void startMotion(int x, int y)
        {
            //Stop Editing Entity and start changing its motion
            bCreatingEntity = false;
            bLaunchingEntity = true;

            newEntity.x_speed = (x - newEntity.x) / 5;
            newEntity.y_speed = (y - newEntity.y) / 5;
        }

        public void updateMotion(int x, int y)
        {
            //Update the current motion vector
            newEntity.x_speed = (x - newEntity.x) / 5;
            newEntity.y_speed = (y - newEntity.y) / 5;
        }

        public void endMotion(int x, int y)
        {
            //Finish Editing its motion and add it to the entity list
            bLaunchingEntity = false;

            //Copy Old E-List + 1
            gfxMutex.WaitOne();
            Entity[] newE = new Entity[entities.Length + 1];
            for (int i = 0; i < entities.Length; i++)
            {
                newE[i] = entities[i];
            }
            newE[entities.Length] = newEntity;
            entities = newE;
            gfxMutex.ReleaseMutex();
        }

        public void startComponent(int x, int y)
        {
            //Add to Entity and Create Component
            bCreatingComponent = true;

            //Make Component
            newComponent = new Component();
            newComponent.type = componentType;
            newComponent.xOff = x - newEntity.x;
            newComponent.yOff = y - newEntity.y;
            newComponent.size = 0;

            //Copy Old C-List + 1
            Component[] newC = new Component[newEntity.c.Length + 1];
            for (int i = 0; i < newEntity.c.Length; i++)
            {
                newC[i] = newEntity.c[i];
            }
            newC[newEntity.c.Length] = newComponent;
            newEntity.c = newC;
        }

        public void updateComponent(int x, int y)
        {
            float xOff = Math.Abs(x - (newEntity.x + newComponent.xOff));
            float yOff = Math.Abs(y - (newEntity.y + newComponent.yOff));

            //Update Size
            switch (newComponent.type)
            {
                case 0:
                    //Circle
                    newComponent.size = (float)Math.Sqrt(xOff * xOff + yOff * yOff);
                    break;
                case 1:
                    //Reserved for Future Use
                    break;
                case 2:
                    //Square
                    newComponent.size = xOff > yOff ? xOff : yOff;
                    break;
            }
            newEntity.c[newEntity.c.Length - 1] = newComponent;
        }

        public void endComponent(int x, int y)
        {
            //Stop Editing the Component and Finalize it
            bCreatingComponent = false;
        }

        private void CircleButton_Click(object sender, EventArgs e)
        {
            SquareButton.ForeColor = Color.Black;
            SquareButton.BackColor = default(Color);
            SquareButton.UseVisualStyleBackColor = true;
            CircleButton.BackColor = Color.Yellow;
            CircleButton.ForeColor = Color.Silver;

            componentType = 0;
        }

        private void SquareButton_Click(object sender, EventArgs e)
        {
            SquareButton.ForeColor = Color.Silver;
            SquareButton.BackColor = Color.Yellow;
            CircleButton.BackColor = default(Color);
            CircleButton.UseVisualStyleBackColor = true;
            CircleButton.ForeColor = Color.Black;

            componentType = 2;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            gfxMutex.WaitOne();
            moveThread.Abort();
            gfxMutex.ReleaseMutex();
            entities = new Entity[0];
            Invalidate();

            moveThread = new Thread(MovementThread);
            moveThread.Start();
        }
        #endregion

        #region Collisions
        #region Entity Movement and Update Thread
        public void MovementThread()
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            long calculationTime = s.ElapsedTicks;
            long lastTime = s.ElapsedMilliseconds;
            long timeDiff = lastTime;
            long lastUpdated = 0;
            lastDrawn = lastTime;

            while (true)
            {
                timeDiff = s.ElapsedMilliseconds - lastTime;
                lastTime += timeDiff;

                //Move Entities
                moveEntities(timeDiff / 1000.0f * speedMod, lastTime);

                //Check Collisions
                gfxMutex.WaitOne();
                calculationTime = s.ElapsedTicks;
                checkCollisions();
                long diff = s.ElapsedTicks - calculationTime;
                gfxMutex.ReleaseMutex();
                Thread.Yield();

                // Running on the UI thread
                if (calculationTime - lastUpdated > 10000000 / 8)
                {
                    lastUpdated = calculationTime;
                    performanceLabel.Invoke((MethodInvoker)delegate
                    {
                        diff = diff / 4 + long.Parse(performanceLabel.Text) * 3 / 4;
                        performanceLabel.Text = diff.ToString();
                    });
                }
                // Back on the worker thread
            }
        }

        private void speedBar_Scroll(object sender, EventArgs e)
        {
            speedMod = speedBar.Value / 5.0f;
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            speedBar.Value = 0;
            speedMod = speedBar.Value / 5.0f;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            speedBar.Value = 5;
            speedMod = speedBar.Value / 5.0f;
        }

        private void ReverseButton_Click(object sender, EventArgs e)
        {
            speedBar.Value = -5;
            speedMod = speedBar.Value / 5.0f;
        }

        public void moveEntities(float m, long now)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].x += entities[i].x_speed * m;
                entities[i].y += entities[i].y_speed * m;
            }
            if (now - lastDrawn > 1000 / 60)
            {
                for (int i = 0; i < entities.Length; i++)
                    invalidateEntity(entities[i]);
                lastDrawn = now;
            }
        }
        #endregion

        #region Collision Checking Functions
        public void checkCollisions()
        {
            //Reset Booleans
            resetEntities();

            #region Bounding Shapes
            if (collisionMethod == 0)
            {
                #region Bruce Force
                if (collisionStrategy == 0)
                {
                    for (int i = 0; i < entities.Length; i++)
                    {
                        for (int j = i + 1; j < entities.Length; j++)
                        {
                            if (BoundingShapesCheck(i, j))
                            {
                                entities[i].colliding = true;
                                entities[j].colliding = true;
                            }
                        }
                    }
                }
                #endregion

                #region Spatial Hashing
                if (collisionStrategy == 2)
                {
                    newGrid();
                    addAllToGrid();
                    //Loop through the Grid until 2 in one
                    foreach (Cell c in grid.Cells)
                    {
                        if (c.eRefs.Length > 1)
                        {
                            //Found 2 or More
                            for (int i = 0; i < c.eRefs.Length; i++)
                            {
                                for (int j = i + 1; j < c.eRefs.Length; j++)
                                {
                                    if (BoundingShapesCheck(c.eRefs[i] - 1, c.eRefs[j] - 1))
                                    {
                                        entities[c.eRefs[i] - 1].colliding = true;
                                        entities[c.eRefs[j] - 1].colliding = true;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region QuadTree
                if (collisionStrategy == 1)
                {
                    //Create Quad-Tree
                    QuadTree qt = new QuadTree(Width, Height, 4);
                    for (int i = 0; i < entities.Length; i++)
                        qt.Insert(entities[i], i);

                    for (int e = 0; e < entities.Length; e++)
                    {
                        int[] posCollisions = qt.GetPossibleCollisions(entities[e]);
                        if (posCollisions.Length > 1)
                        {
                            //Found 2 or More
                            for (int i = 0; i < posCollisions.Length; i++)
                            {
                                if (posCollisions[i] != e)
                                {
                                    //Two Different Entitities
                                    if (BoundingShapesCheck(posCollisions[i], e))
                                    {
                                        entities[posCollisions[i]].colliding = true;
                                        entities[e].colliding = true;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion

            #region Pixel Perfect
            else if (collisionMethod == 1)
            {
                newCanvas();
                #region Bruce Force
                if (collisionStrategy == 0)
                {
                    for (int i = 0; i < entities.Length; i++)
                    {
                        drawVEntity(entities[i], i);
                    }
                }
                #endregion

                #region Spatial Hashing
                if (collisionStrategy == 2)
                {
                    bool[] entitiesDrawn = new bool[entities.Length];
                    newGrid();
                    addAllToGrid();
                    //Loop through the Grid until 2 in one
                    foreach (Cell c in grid.Cells)
                    {
                        if (c.eRefs.Length > 1)
                        {
                            //Found 2 or More
                            for (int i = 0; i < c.eRefs.Length; i++)
                            {
                                if (entitiesDrawn[c.eRefs[i] - 1] == false)
                                {
                                    entitiesDrawn[c.eRefs[i] - 1] = true;
                                    drawVEntity(entities[c.eRefs[i] - 1], c.eRefs[i] - 1);
                                }
                            }
                        }
                    }

                }
                #endregion

                #region QuadTree
                if (collisionStrategy == 1)
                {
                    bool[] entitiesDrawn = new bool[entities.Length];

                    //Create Quad-Tree
                    QuadTree qt = new QuadTree(Width, Height, 4);
                    for (int i = 0; i < entities.Length; i++)
                        qt.Insert(entities[i], i);

                    for (int e = 0; e < entities.Length; e++)
                    {
                        int[] posCollisions = qt.GetPossibleCollisions(entities[e]);
                        if (posCollisions.Length > 1)
                        {
                            //Found 2 or More
                            for (int i = 0; i < posCollisions.Length; i++)
                            {
                                if (posCollisions[i] != e)
                                {
                                    if (entitiesDrawn[posCollisions[i]] == false)
                                    {
                                        entitiesDrawn[posCollisions[i]] = true;
                                        drawVEntity(entities[posCollisions[i]], posCollisions[i]);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion
        }

        public void resetEntities()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].colliding = false;
            }
        }

        private void BruceForceButton_Click(object sender, EventArgs e)
        {
            BruceForceButton.ForeColor = Color.Silver;
            BruceForceButton.BackColor = Color.Yellow;
            SpatialHashingButton.BackColor = default(Color);
            SpatialHashingButton.UseVisualStyleBackColor = true;
            SpatialHashingButton.ForeColor = Color.Black;
            QuadTreeButton.BackColor = default(Color);
            QuadTreeButton.UseVisualStyleBackColor = true;
            QuadTreeButton.ForeColor = Color.Black;

            collisionStrategy = 0;
        }
        #endregion

        //Narrow Collision Methods
        #region PixelPerfect Collision Checking
        public void newCanvas()
        {
            vCanvas = new int[(int)(Height * vCanvasScale), (int)(Width * vCanvasScale)];
        }

        public bool drawVCircle(float xx, float yy, float radius, int entityNumber)
        {
            int x = (int)(xx * vCanvasScale);
            int y = (int)(yy * vCanvasScale);
            float r = radius * vCanvasScale;
            int rR = (int)Math.Round(r);
            int vC_width = vCanvas.GetLength(1);
            int vC_height = vCanvas.GetLength(0);
            bool hit = false;

            for (int iy = -rR; iy <= rR; iy++)
            {
                if (iy + y >= vC_height || iy + y < 0)
                    continue;

                for (int ix = -rR; ix <= rR; ix++)
                {
                    if (ix + x >= vC_width || ix + x < 0)
                        continue;

                    //If point inside radius
                    if (Math.Sqrt(ix * ix + iy * iy) < radius)
                    {
                        if (!(vCanvas[iy + y, ix + x] == 0 || vCanvas[iy + y, ix + x] - 1 == entityNumber))
                        {
                            entities[vCanvas[iy + y, ix + x] - 1].colliding = true;
                            entities[entityNumber].colliding = true;
                            hit = true;
                        }
                        vCanvas[iy + y, ix + x] = entityNumber + 1;
                    }
                }
            }
            return hit;
        }

        public bool drawVSquare(float xx, float yy, float radius, int entityNumber)
        {
            int x = (int)(xx * vCanvasScale);
            int y = (int)(yy * vCanvasScale);
            float r = radius * vCanvasScale;
            int rR = (int)Math.Round(r);
            int vC_width = vCanvas.GetLength(1);
            int vC_height = vCanvas.GetLength(0);
            bool hit = false;

            for (int iy = -rR; iy <= rR; iy++)
            {
                if (iy + y >= vC_width || iy + y < 0)
                    continue;

                for (int ix = -rR; ix <= rR; ix++)
                {
                    if (ix + x >= vC_width || ix + x < 0)
                        continue;

                    if (!(vCanvas[iy + y, ix + x] == 0 || vCanvas[iy + y, ix + x] - 1 == entityNumber))
                    {
                        entities[vCanvas[iy + y, ix + x] - 1].colliding = true;
                        entities[entityNumber].colliding = true;
                        hit = true;
                    }
                    vCanvas[iy + y, ix + x] = entityNumber + 1;
                }
            }
            return hit;
        }
        
        public bool drawVEntity(Entity e, int entityNumber)
        {
            bool hit = false;
            foreach (Component c in e.c)
            {
                switch (c.type)
                {
                    case 0:
                        //Circle
                        if (c.size == 0) { break; }
                        if (drawVCircle(e.x + c.xOff, e.y + c.yOff, c.size, entityNumber))
                            hit = true;
                        break;
                    case 1:
                        //Reserved for Future Use
                        if (c.size == 0) { break; }
                        break;
                    case 2:
                        //Square
                        if (c.size == 0) { break; }
                        if (drawVSquare(e.x + c.xOff, e.y + c.yOff, c.size, entityNumber))
                            hit = true;
                        break;
                }
            }
            return hit;
        }

        public bool PixelPerfectCheck(int eRef1, int eRef2)
        {
            drawVEntity(entities[eRef1], eRef1);
            return drawVEntity(entities[eRef2], eRef2);
        }

        private void PixelPerfectButton_Click(object sender, EventArgs e)
        {
            collisionMethod = 1;
            PixelPerfectButton.ForeColor = Color.Silver;
            PixelPerfectButton.BackColor = Color.Yellow;
            BoundingBoxButton.BackColor = default(Color);
            BoundingBoxButton.UseVisualStyleBackColor = true;
            BoundingBoxButton.ForeColor = Color.Black;
        }
        #endregion

        #region Bounding Shapes
        public static bool BoundingSphereCheck(Component A, Component B, float diffX, float diffY)
        {
            float distX = diffX + B.xOff - A.xOff;
            float distY = diffY + B.yOff - A.yOff;

            double dist = Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));

            return dist < (A.size + B.size);
        }

        public static bool BoundingBoxCheck(Component A, Component B, float diffX, float diffY)
        {
            BoundingBox BoxA = new BoundingBox(A, 0, 0);
            BoundingBox BoxB = new BoundingBox(B, diffX + B.xOff - A.xOff, diffY + B.yOff - A.yOff);

            float d1x = BoxB.minX - BoxA.maxX;
            float d1y = BoxB.minY - BoxA.maxY;
            float d2x = BoxA.minX - BoxB.maxX;
            float d2y = BoxA.minY - BoxB.maxY;

            if (d1x > 0.0f || d1y > 0.0f)
                return false;

            if (d2x > 0.0f || d2y > 0.0f)
                return false;

            return true;
        }

        public bool BoundsCheck(Component A, Component B, float diffX, float diffY)
        {
            if (A.type == 0 || B.type == 0)
            {
                return BoundingSphereCheck(A, B, diffX, diffY);
            }
            else
            {
                return BoundingBoxCheck(A, B, diffX, diffY);
            }
        }

        public bool BoundingShapesCheck(int eRef1, int eRef2)
        {
            Entity Ent1 = entities[eRef1];
            Entity Ent2 = entities[eRef2];
            float diffX = Ent2.x - Ent1.x;
            float diffY = Ent2.y - Ent1.y;

            foreach (Component comp1 in Ent1.c)
            {
                foreach (Component comp2 in Ent2.c)
                {
                    if (BoundsCheck(comp1, comp2, diffX, diffY))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool BoundingShapesCheck(Entity Ent1, Entity Ent2)
        {
            float diffX = Ent2.x - Ent1.x;
            float diffY = Ent2.y - Ent1.y;

            foreach (Component comp1 in Ent1.c)
            {
                foreach (Component comp2 in Ent2.c)
                {
                    if (BoundsCheck(comp1, comp2, diffX, diffY))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void BoundingBoxButton_Click(object sender, EventArgs e)
        {
            collisionMethod = 0;
            BoundingBoxButton.ForeColor = Color.Silver;
            BoundingBoxButton.BackColor = Color.Yellow;
            PixelPerfectButton.BackColor = default(Color);
            PixelPerfectButton.UseVisualStyleBackColor = true;
            PixelPerfectButton.ForeColor = Color.Black;
        }
        #endregion

        //Wide Collision Strategies
        #region Spatial Hashing
        public void newGrid()
        {
            Grid g = new Grid();
            g.CellSize = 75;

            g.Cells = new Cell[(int)Math.Ceiling((float)Height / g.CellSize), (int)Math.Ceiling((float)Width / g.CellSize)];
            for (int j = 0; j < g.Cells.GetLength(0); j++)
            {
                for (int i = 0; i < g.Cells.GetLength(1); i++)
                {
                    g.Cells[j, i].eRefs = new int[0];
                }
            }
            grid = g;
        }

        public void addAllToGrid()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                addEntityToGrid(i);
            }
        }
        
        public int[] hashPoint(int x, int y)
        {
            int[] hash = new int[2];
            hash[0] = x / grid.CellSize;
            hash[1] = y / grid.CellSize;
            
            if (hash[1] < 0 || hash[1] >= grid.Cells.GetLength(0) || hash[0] < 0 || hash[0] >= grid.Cells.GetLength(1))
            {
                hash[0] = -1;
                hash[1] = -1;
            }
            return hash;
        }

        public bool badHash(int[] hash)
        {
            return hash[0] == -1;
        }

        public bool EntityIsInCell(int[] hash, int entityNumber)
        {
            int i = hash[0];
            int j = hash[1];

            for (int t = 0; t < grid.Cells[j, i].eRefs.Length; t++)
            {
                if (grid.Cells[j, i].eRefs[t] == entityNumber + 1)
                    return true;
            }
            return false;
        }

        public void addEntityToGrid(int eRef)
        {
            for (int i = 0; i < entities[eRef].c.Length; i++)
            {
                addComponentToGrid(entities[eRef].c[i], eRef);
            }
        }

        public void addComponentToGrid(Component c, int eRef)
        {
            int rR = (int)Math.Round(c.size);
            int x = (int)Math.Round(entities[eRef].x + c.xOff);
            int y = (int)Math.Round(entities[eRef].y + c.yOff);

            switch (c.type)
            {
                case 0:
                    //Circle
                    rR = (int)Math.Round(c.size * Math.Sqrt(2));
                    for (int iy = -rR; iy <= rR; iy += grid.CellSize)
                    {
                        for (int ix = -rR; ix <= rR; ix += grid.CellSize)
                        {
                            addEntityToCell(hashPoint(x + ix, y + iy), eRef);
                        }
                    }
                    addEntityToCell(hashPoint(x, y + rR), eRef);
                    addEntityToCell(hashPoint(x + rR, y), eRef);
                    addEntityToCell(hashPoint(x + rR, y + rR), eRef);
                    break;
                case 1:
                    //Reserved for Future Use
                    break;
                case 2:
                    //Square
                    for (int iy = -rR; iy <= rR; iy += grid.CellSize)
                    {
                        for (int ix = -rR; ix <= rR; ix += grid.CellSize)
                        {
                            addEntityToCell(hashPoint(x + ix, y + iy), eRef);
                        }
                    }
                    addEntityToCell(hashPoint(x, y + rR), eRef);
                    addEntityToCell(hashPoint(x + rR, y), eRef);
                    addEntityToCell(hashPoint(x + rR, y + rR), eRef);
                    break;
            }
        }

        public void removeEntityFromGrid(int eRef)
        {
            for (int i = 0; i < entities[eRef].c.Length; i++)
            {
                removeComponentFromGrid(entities[eRef].c[i], eRef);
            }
        }

        public void removeComponentFromGrid(Component c, int eRef)
        {
            int rR = (int)Math.Round(c.size);
            int x = (int)Math.Round(entities[eRef].x + c.xOff);
            int y = (int)Math.Round(entities[eRef].y + c.yOff);

            switch (c.type)
            {
                case 0:
                    //Circle
                    rR = (int)Math.Round(c.size * Math.Sqrt(2));
                    for (int iy = -rR; iy <= rR; iy += grid.CellSize)
                    {
                        for (int ix = -rR; ix <= rR; ix += grid.CellSize)
                        {
                            removeEntityFromCell(hashPoint(x + ix, y + iy), eRef);
                        }
                    }
                    removeEntityFromCell(hashPoint(x, y + rR), eRef);
                    removeEntityFromCell(hashPoint(x + rR, y), eRef);
                    removeEntityFromCell(hashPoint(x + rR, y + rR), eRef);
                    break;
                case 1:
                    //Reserved for Future Use
                    break;
                case 2:
                    //Square
                    for (int iy = -rR; iy <= rR; iy += grid.CellSize)
                    {
                        for (int ix = -rR; ix <= rR; ix += grid.CellSize)
                        {
                            removeEntityFromCell(hashPoint(x + ix, y + iy), eRef);
                        }
                    }
                    removeEntityFromCell(hashPoint(x, y + rR), eRef);
                    removeEntityFromCell(hashPoint(x + rR, y), eRef);
                    removeEntityFromCell(hashPoint(x + rR, y + rR), eRef);
                    break;
            }
        }

        public void addEntityToCell(int[] hash, int entityNumber)
        {
            int i = hash[0];
            int j = hash[1];

            if (badHash(hash) || EntityIsInCell(hash, entityNumber))
                return;

            int[] eRefsNew = new int[grid.Cells[j, i].eRefs.Length + 1];

            for (int t = 0; t < grid.Cells[j, i].eRefs.Length; t++)
            {
                eRefsNew[t] = grid.Cells[j, i].eRefs[t];
            }
            eRefsNew[grid.Cells[j, i].eRefs.Length] = entityNumber + 1;
            grid.Cells[j, i].eRefs = eRefsNew;
        }

        public void removeEntityFromCell(int[] hash, int entityNumber)
        {
            int i = hash[0];
            int j = hash[1];

            if (badHash(hash) || !EntityIsInCell(hash, entityNumber))
                return;

            int[] eRefsNew = new int[grid.Cells[j, i].eRefs.Length - 1];
            int index = 0;

            for (int t = 0; t < grid.Cells[j, i].eRefs.Length; t++)
            {
                if (grid.Cells[j, i].eRefs[t] != entityNumber + 1)
                    eRefsNew[index++] = grid.Cells[j, i].eRefs[t];
            }
            grid.Cells[j, i].eRefs = eRefsNew;
        }

        private void SpatialHashingButton_Click(object sender, EventArgs e)
        {
            SpatialHashingButton.ForeColor = Color.Silver;
            SpatialHashingButton.BackColor = Color.Yellow;
            BruceForceButton.BackColor = default(Color);
            BruceForceButton.UseVisualStyleBackColor = true;
            BruceForceButton.ForeColor = Color.Black;
            QuadTreeButton.BackColor = default(Color);
            QuadTreeButton.UseVisualStyleBackColor = true;
            QuadTreeButton.ForeColor = Color.Black;

            collisionStrategy = 2;
        }
        #endregion

        #region QuadTree
        private void QuadTreeButton_Click(object sender, EventArgs e)
        {
            QuadTreeButton.ForeColor = Color.Silver;
            QuadTreeButton.BackColor = Color.Yellow;
            SpatialHashingButton.BackColor = default(Color);
            SpatialHashingButton.UseVisualStyleBackColor = true;
            SpatialHashingButton.ForeColor = Color.Black;
            BruceForceButton.BackColor = default(Color);
            BruceForceButton.UseVisualStyleBackColor = true;
            BruceForceButton.ForeColor = Color.Black;

            collisionStrategy = 1;
        }
        #endregion
        #endregion

        #region Loading/Saving
        private void SaveButton_Click(object sender, EventArgs evt)
        {
            //Stop Motion
            speedMod = 0;
            speedBar.Value = 0;

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Entity List File (*.EL)|*.EL;";
            if (save.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            FileStream fs = File.Create(save.FileName);
            BinaryWriter writer = new BinaryWriter(fs);

            //# Of Entities
            writer.Write((int)entities.Length);

            //Foreach Entity
            foreach (Entity e in entities)
            {
                //# of Components
                writer.Write((int)e.c.Length);
                //Foreach Entity
                foreach (Component c in e.c)
                {
                    writer.Write((int)c.type);
                    writer.Write((int)c.size);
                    writer.Write((int)c.xOff);
                    writer.Write((int)c.yOff);
                }
                writer.Write((int)e.x);
                writer.Write((int)e.y);
                writer.Write((int)e.x_speed);
                writer.Write((int)e.y_speed);
                writer.Write((int)e.color.ToArgb());
            }

            fs.Close();
        }

        private void LoadButton_Click(object sender, EventArgs evt)
        {
            //Stop Thread
            gfxMutex.WaitOne();
            moveThread.Abort();
            gfxMutex.ReleaseMutex();

            //Stop Motion
            speedMod = 0;
            speedBar.Value = 0;

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Entity List File (*.EL)|*.EL;";
            if (open.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            FileStream fs = File.Open(open.FileName, FileMode.Open);
            BinaryReader reader = new BinaryReader(fs);

            //# Of Entities
            Entity[] e = new Entity[reader.ReadInt32() + entities.Length];

            //Add copy old entities back
            for (int i = 0; i < entities.Length; i++)
                e[i] = entities[i];

            //Foreach Entity
            for (int i = 0; i < e.Length - entities.Length; i++)
            {
                //# of Components
                e[i + entities.Length].c = new Component[reader.ReadInt32()];
                //Foreach Entity
                for (int j = 0; j < e[i + entities.Length].c.Length; j++)
                {
                    e[i + entities.Length].c[j].type = reader.ReadInt32();
                    e[i + entities.Length].c[j].size = reader.ReadInt32();
                    e[i + entities.Length].c[j].xOff = reader.ReadInt32();
                    e[i + entities.Length].c[j].yOff = reader.ReadInt32();
                }
                e[i + entities.Length].x = reader.ReadInt32();
                e[i + entities.Length].y = reader.ReadInt32();
                e[i + entities.Length].x_speed = reader.ReadInt32();
                e[i + entities.Length].y_speed = reader.ReadInt32();
                e[i + entities.Length].color = Color.FromArgb(reader.ReadInt32());
            }
            entities = e;

            fs.Close();
            Invalidate();
            moveThread = new Thread(MovementThread);
            moveThread.Start();
        }
        #endregion
    }
}
