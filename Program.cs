using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Platform
{
    static class Program
    {
        static float t = 0;
        static Sprite sprite;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            World world = null;
            Mario mario = null;

            using (var game = new GameWindow())
            {
                game.Load += (sender, e) =>
                {
                    // setup settings, load textures, sounds
                    game.VSync = VSyncMode.On;

                    world = new World();
                    mario = new Mario(world);
                };

                game.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, game.Width, game.Height);
                };


                float dir = 0.01f;

                game.UpdateFrame += (sender, e) =>
                {
                    if (world != null)
                        world.Update(1.0f / 60.0f);

                    if (sprite == null)
                    {
                        sprite = new Sprite(Utils.DataFile("textures/mario-3.GIF"), 5, 9);
                    }

                    sprite.AnimationX = (int)(Math .Abs(sprite.X)*7) % 3;
                    sprite.X += dir;

                    if (Math.Abs(sprite.X + dir) > 2)
                    {
                        dir = -dir;
                        sprite.FlipX = !sprite.FlipX;
                    }

                    t += 1.0f / 60.0f;
                };

                game.KeyDown += (object sender, KeyboardKeyEventArgs e) => {
                    if (e.Key == Key.Escape)
                        game.Exit();

                    switch(e.Key)
                    {
                        case Key.Left:
                            mario.WalkBackward = true;
                            break;

                        case Key.Right:
                            mario.WalkForward = true;
                            break;
                    }
                };

                game.KeyUp += (object sender, KeyboardKeyEventArgs e) => {
                    if (e.Key == Key.Escape)
                        game.Exit();

                    switch (e.Key)
                    {
                        case Key.Left:
                            mario.WalkBackward = false;
                            break;

                        case Key.Right:
                            mario.WalkForward = false;
                            break;
                    }
                };

                game.RenderFrame += (sender, e) =>
                {
                    // render graphics
                    GL.ClearColor(Color4.Black);
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                    GL.Enable(EnableCap.Blend);

                    GL.Enable(EnableCap.LineSmooth);

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();
                    GL.Ortho(-1.0, 2.0, -2.0, 2.0, 0.0, 4.0);
                    
                    GL.Begin(PrimitiveType.Triangles);
                        GL.Color4(Color4.MidnightBlue);
                        GL.Vertex2(-1.0f, 1.0f);
                        GL.Color4(Color4.SpringGreen);
                        GL.Vertex2(Math.Cos(Math.Sin(t*2)*3.5), Math.Cos(Math.Cos(t * 1.7) * 3.5));
                        GL.Color4(Color4.Ivory);
                        GL.Vertex2(1.0f, 1.0f);
                    GL.End();

                    if (sprite != null)
                        sprite.Draw();

                    if(world != null)
                        world.Draw();

                    game.SwapBuffers();
                };

                // Run the game at 60 updates per second
                game.Run(60.0);
            }
        }
        
    }
}
