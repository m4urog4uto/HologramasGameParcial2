using System;
using System.Media;
using Tao.Sdl;
using System.Timers;

public struct GameObject
{
    public int posX;
    public int posY;
    public int width;
    public int height;
    public int speed;
    public int lifeBar;
    public bool isAtacking;
    public bool isRight;
    public bool isActive;
    public Image CharacterKO;
    public Image iconCharacter;
    public Image imageRunRight;
    public Image imageAttackRight;
    public Image imageRunLeft;
    public Image imageAttackLeft;
}

namespace MyGame
{
    
    class Program
    {
        // GAME CONFIGURATION
        static Font fontTextInteraction;
        static Font fontStartGame;
        static Font mainCharacterLife;
        static Image startGame = Engine.LoadImage("assets/scene/start_game.png");
        static Image inGame = Engine.LoadImage("assets/scene/scene.png");
        static int gameSteps = 0;
        static Random random = new Random();
        static Timer timer = new Timer(2000);

        // MUSIC
        static SoundPlayer startGameMusic = new SoundPlayer("assets/music/start_game_music.wav");
        static SoundPlayer inGameMusic = new SoundPlayer("assets/music/in_game_music.wav");

        // VICTORY AND DEFEAT IMAGES
        static Image victory = Engine.LoadImage("assets/scene/victory/victory.png");
        static Image defeat = Engine.LoadImage("assets/scene/defeat/defeat.png");

        // LIMITS CHARACTERS MOVEMENTS
        static int limitMinStreetX = 0;
        static int limitMaxStreetX = 1050;

        static int limitMinStreetY = 300;
        static int limitMaxStreetY = 485;


        static GameObject player;
        static GameObject[] enemy = new GameObject[3];

        static int enemyQuantity;

        static void InitializedGameObjects()
        {
            player.posX = limitMinStreetX;
            player.posY = limitMaxStreetY;
            player.width = 90;
            player.height = 150;
            player.isRight = true;
            player.lifeBar = 100;
            player.speed = 6;
            player.iconCharacter = Engine.LoadImage("assets/characters/main_character/HUD/HUD_Icon.png");
            player.imageRunRight = Engine.LoadImage("assets/characters/main_character/movements/main_attack_right_01.png");
            player.imageAttackRight = Engine.LoadImage("assets/characters/main_character/movements/main_attack_right_02.png");
            player.imageRunLeft = Engine.LoadImage("assets/characters/main_character/movements/main_attack_left_01.png");
            player.imageAttackLeft = Engine.LoadImage("assets/characters/main_character/movements/main_attack_left_02.png");
            player.CharacterKO = Engine.LoadImage("assets/characters/main_character/main_KO.png");
            player.isAtacking = false;
            enemyQuantity = enemy.Length;


            for (int i = 0; i < enemy.Length; i++)
            {
                enemy[i].isActive = true;
                enemy[i].posY = random.Next(limitMinStreetY, limitMaxStreetY);
                enemy[i].posX = random.Next(player.posX + 500, limitMaxStreetX);
                enemy[i].width = 60;
                enemy[i].height = 100;
                enemy[i].lifeBar = 100;
                enemy[i].imageRunRight = Engine.LoadImage("assets/characters/hologram/enemy.png");
                enemy[i].imageAttackLeft = Engine.LoadImage("assets/characters/hologram/enemy_attack_left.png");
                enemy[i].speed = random.Next(4, 9);
                enemy[i].CharacterKO = Engine.LoadImage("assets/characters/hologram/enemy_KO.png");
                enemy[i].isAtacking = false;
            }
        }

        static void Main(string[] args)
        {
            Engine.Initialize();
            InitializedGameObjects();
            startGameMusic.Play();

            fontStartGame = Engine.LoadFont("assets/fonts/BowlbyOneSC.ttf", 40);
            fontTextInteraction = Engine.LoadFont("assets/fonts/BowlbyOneSC.ttf", 20);
            mainCharacterLife = Engine.LoadFont("assets/fonts/BowlbyOneSC.ttf", 40);

            while (true)
            {
                
                CheckInputs();
                Update();
                Render();
                Sdl.SDL_Delay(20);  
            }
        }

        static void CheckInputs()
        {
            // Movements Keys
            if (Engine.KeyPress(Engine.KEY_A))
            {
                if (player.lifeBar != 0 && player.posX > limitMinStreetX)
                {
                    player.isRight = false;
                    player.posX -= player.speed;
                }
            }

            if (Engine.KeyPress(Engine.KEY_D))
            {
                if (player.lifeBar != 0 && player.posX < limitMaxStreetX)
                {
                    player.isRight = true;
                    player.posX += player.speed;
                }
            }

            if (Engine.KeyPress(Engine.KEY_W))
            {
                if (player.lifeBar != 0 && player.posY > limitMinStreetY)
                {
                    player.posY -= player.speed;
                }
            }

            if (Engine.KeyPress(Engine.KEY_S))
            {
                if (player.lifeBar != 0 && player.posY < limitMaxStreetY)
                {
                    player.posY += player.speed;
                }
            }

            // Actions
            if (Engine.KeyPress(Engine.KEY_F))
            {
                if (player.lifeBar != 0)
                {
                    player.isAtacking = true;
                }
            }

            if (Engine.KeyPress(Engine.KEY_ESC))
            {
                Environment.Exit(0);
            }

            // Exit
            if (Engine.KeyPress(Engine.KEY_ESP))
            {
                if (gameSteps != 1)
                {
                    startGameMusic.Stop();
                    inGameMusic.PlayLooping();
                    InitializedGameObjects();
                    gameSteps = 1;
                }
            }
        }

        static void timerForEndScene()
        {
            timer.Elapsed += (sender, e) =>
            {
                timer.Stop();
                gameSteps = 3;
            };
            timer.AutoReset = false;
            timer.Start();
        }

        static void MainCharacterProperties()
        {
            Engine.Draw(player.iconCharacter, 0, 0);
            Engine.DrawText("vida: " + player.lifeBar + "%", 200, 60, 248, 37, 248, mainCharacterLife);

            if (player.lifeBar == 0)
            {
                Engine.Draw(player.CharacterKO, player.posX, player.posY + 50);
                timer.Elapsed += (sender, e) =>
                {
                    timer.Stop();
                    gameSteps = 4;
                };
                timer.AutoReset = false;
                timer.Start();
            } else
            {
                if (player.isAtacking)
                {
                    if (player.isRight)
                    {
                        Engine.Draw(player.imageAttackRight, player.posX, player.posY);
                    }
                    else
                    {
                        Engine.Draw(player.imageAttackLeft, player.posX, player.posY);
                    }
                    player.isAtacking = false;
                }
                else
                {
                    if (player.isRight)
                    {
                        Engine.Draw(player.imageRunRight, player.posX, player.posY);
                    }
                    else
                    {
                        Engine.Draw(player.imageRunLeft, player.posX, player.posY);
                    }
                }
            }
        }
        static void ShowEnemys()
        {
            for (int i = 0; i < enemy.Length; i++)
            {
                
                if (enemy[i].isActive)
                {
                    if (enemy[i].isAtacking)
                    {
                        Engine.Draw(enemy[0].imageAttackLeft, enemy[i].posX, enemy[i].posY);
                    }
                    else
                    {
                        Engine.Draw(enemy[0].imageRunRight, enemy[i].posX, enemy[i].posY);
                    }
                }
                else
                {
                    Engine.Draw(enemy[0].CharacterKO, enemy[i].posX, enemy[i].posY + 150);
                    if (enemyQuantity == 0)
                    {
                        timer.Elapsed += (sender, e) =>
                        {
                            timer.Stop();
                            gameSteps = 3;
                        };
                        timer.AutoReset = false;
                        timer.Start();
                    }
                }
            }
        }

        static void attackPlayer() 
        {
            if (player.lifeBar != 0)
            {
                player.lifeBar = player.lifeBar - 1;
            }
        }

        static void Update()
        {
            if (gameSteps == 1)
            {
                // Enemy walking
                for (int i = 0; i < enemy.Length; i++)
                {
                    if (enemy[i].isActive)
                    {
                        enemy[i].posX += enemy[i].speed;

                        if (enemy[i].posX >= limitMaxStreetX || enemy[i].posX <= limitMinStreetX)
                        {
                            enemy[i].speed = enemy[i].speed * -1;
                        }
                    }
                }

                // Check Collision
                for (int i = 0; i < enemy.Length; i++)
                {
                    if (enemy[i].isActive)
                    {
                        if (checkCollision(player, enemy[i]))
                        {
                            enemy[i].isAtacking = true;

                            if (player.isAtacking)
                            {
                                enemy[i].isActive = false;
                                enemyQuantity = enemyQuantity - 1;
                            }
                            else
                            {
                                attackPlayer();
                            }
                        } else
                        {
                            enemy[i].isAtacking = false;
                        }
                    }

                }
            }
        }

        static void EndScene(Image img, byte r, byte g, byte b)
        {
            Engine.Draw(img, 0, 0);
            
            Engine.DrawText("¡Presione 'Espacio' para jugar nuevamente!", 600, 430, r, g, b, fontTextInteraction);
            Engine.DrawText("¡Presione 'Escape' para salir!", 600, 460, r, g, b, fontTextInteraction);
        }

        static bool checkCollision(GameObject a, GameObject b)
        {
            return a.posX < b.posX + b.width &&
                   a.posX + a.width > b.posX &&
                   a.posY < b.posY + b.height &&
                   a.posY + a.height > b.posY;
        }

        static void Render()
        {
            Engine.Clear();
            switch (gameSteps)
            {
                case 0:
                    Engine.Draw(startGame, 0, 0);
                    break;
                case 1:
                    Engine.Draw(inGame, 0, 0);
                    MainCharacterProperties();
                    ShowEnemys();
                    break;
                case 3:
                    EndScene(victory, 248, 37, 248);
                    break;
                case 4:
                    EndScene(defeat, 255, 0, 0);
                    break;

            }

            Engine.Show();
        }
    }
}