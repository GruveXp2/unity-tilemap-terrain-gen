using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGeneration : MonoBehaviour
{
    [Range(8, 512)] [SerializeField] int map_width;
    [Range(0, 128)] [SerializeField] int map_height;
    //[SerializeField] GameObject dirt, grass, stone, dense_stone;
    [SerializeField] Tilemap DirtTilemap, StoneTilemap;
    [SerializeField] Tile dirt, grass, stone, dense_stone, brick, brick_slab, brick_slab2, chest, air;
    [SerializeField] Tile stone_coal_ore, stone_copper_ore, stone_iron_ore;
    [Range(2, 128)] [SerializeField] float heightgen, smoothnessgen;
    [Range(1, 4)] [SerializeField] float layer_height;
    [Range(-128, 128)] [SerializeField] int seed;
    [Range(0, 1)] [SerializeField] float coal_size, coal_scale, copper_size, copper_scale, iron_size, iron_scale, dungeon_scale, dungeon2_scale;
    [Range(0.9f, 1)] [SerializeField] float dungeon_size, dungeon2_size;
    int[] dungeon_components =
    {
        1, 1, 1, 1, 1, 1, 1,
        1, 0, 0, 0, 0, 0, 1,
        1, 0, 0, 0, 0, 0, 1,
        1, 0, 0, 2, 0, 0, 1,
        1, 0, 0, 1, 0, 0, 1,
        1, 0, 0, 1, 0, 0, 1,
        1, 1, 1, 1, 1, 1, 1
    };
    // [,,]   [[ [coords],[extra data] ],[dungeon2],[dungeon3]]
    // 0=void(dont delete an blocks)   1=air   2..= other blocks(2: brick, 3: slab, 4: slab2, 5: chest)
    int[][,][] dungeon2 =
    {//dungeon
        new[,] {   //up    (0)
            {//dungeon1
                new[] {//blocks
                    0
                },
                new[] {//size
                    0
                },
                new[] {//entrance (how much to move the pointer for it to be at 0, 0 local at the new structure)
                    0
                },
                new[] {//exits
                    0
                }
            }       //dungeon1
        }, //up    (0)
        new[,] {   //right (1)
            {//dungeon1
                new[] {//blocks
                    2, 2, 2, 2, 2,
                    1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1,
                    2, 2, 2, 2, 2
                },
                new[] {//size
                    5, 5
                },
                new[] {//entrance
                    0, -1
                },
                new[] {//exits
                    1, 5, 1
                }
            },       //dungeon1
            {//dungeon2
                new[] {//blocks
                    2, 2, 2, 0, 0,
                    1, 1, 1, 2, 0,
                    1, 1, 1, 0, 0,
                    1, 1, 0, 0, 0,
                    2, 2, 2, 0, 2
                },
                new[] {//size
                    5, 5
                },
                new[] {//entrance
                    0, -1
                },
                new[] {//exits
                    9, 0, 0
                }
            },       //dungeon2
            {       //dungeon3
                new[] {//blocks
                    2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    4, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0,
                    1, 1, 4, 2, 2, 0, 0, 0, 0, 0, 0,
                    1, 1, 1, 1, 4, 2, 2, 0, 0, 0, 0,
                    3, 1, 1, 1, 1, 1, 4, 2, 2, 0, 0,
                    2, 2, 3, 1, 1, 1, 1, 1, 4, 2, 2,
                    0, 0, 2, 2, 3, 1, 1, 1, 1, 1, 4,
                    0, 0, 0, 0, 2, 2, 3, 1, 1, 1, 1,
                    0, 0, 0, 0, 0, 0, 2, 2, 3, 1, 1,
                    0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 3,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2
                },
                new[] {//size
                    11, 11
                },
                new[] {//entrance
                    0, -1
                },
                new[] {//exits
                    1, 11, -5
                }
            },       //dungeon3
            {       //dungeon4
                new[] {//blocks
                    2, 2, 2, 2, 2,
                    1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1,
                    2, 4, 1, 4, 2,
                    2, 1, 1, 1, 2,
                    2, 1, 1, 1, 2,
                    2, 1, 1, 1, 2,
                    2, 1, 1, 1, 2,
                    2, 1, 1, 1, 2,
                    2, 1, 1, 1, 2,
                    2, 1, 1, 1, 2,
                    2, 1, 1, 1, 2,
                    1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1,
                    2, 2, 2, 2, 2
                },
                new[] {//size
                    5, 17
                },
                new[] {//entrance
                    0, -1
                },
                new[] {//exits
                    1, 5, 1, 1, 5, -11, 3, 0, -11
                }
            }       //dungeon4
        }, //right (1)
        new[,] {   //down  (2)
            {         //dungeon1
                new[]
                {//blocks
                    0
                },
                new[] {//size
                    0
                },
                new[] {//entrance
                    0
                },
                new[] {//exits
                    0
                }
            }       //dungeon1
        }, //down  (2)
        new[,] {   //left  (3)
            {         //dungeon1
                new[]
                {//blocks
                    2, 2, 2, 2, 2,
                    1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1,
                    2, 2, 2, 2, 2
                },
                new[] {//size
                    5, 5
                },
                new [] {//entrance
                    -5, -1
                },
                new[] {//exits
                    3, 0, 1
                }
            },      //dungeon1
            {         //dungeon2
                new[]
                {//blocks
                    0, 0, 2, 2, 2,
                    0, 2, 1, 1, 1,
                    0, 0, 1, 1, 1,
                    0, 0, 0, 1, 1,
                    2, 0, 2, 2, 2
                },
                new[] {//size
                    5, 5
                },
                new [] {//entrance
                    -5, -1
                },
                new[] {//exits
                    9, 0, 0
                }
            },     //dungeon2
            {         //dungeon3
                new[]
                {//blocks
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2,
                    0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 4,
                    0, 0, 0, 0, 0, 0, 2, 2, 4, 1, 1,
                    0, 0, 0, 0, 2, 2, 4, 1, 1, 1, 1,
                    0, 0, 2, 2, 4, 1, 1, 1, 1, 1, 3,
                    2, 2, 4, 1, 1, 1, 1, 1, 3, 2, 2,
                    4, 1, 1, 1, 1, 1, 3, 2, 2, 0, 0,
                    1, 1, 1, 1, 3, 2, 2, 0, 0, 0, 0,
                    1, 1, 3, 2, 2, 0, 0, 0, 0, 0, 0,
                    3, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0,
                    2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                },
                new[] {//size
                    11, 11
                },
                new [] {//entrance
                    -11, -1
                },
                new[] {//exits
                    3, 0, -5
                }
            },     //dungeon3
            {         //dungeon4
                new[]
                {//blocks
                    2, 2, 2, 2, 2,
                    1, 1, 1, 1, 2,
                    1, 1, 1, 1, 2,
                    1, 1, 1, 1, 2,
                    2, 4, 1, 4, 2,
                    2, 1, 1, 1, 2,
                    1, 1, 1, 1, 2,
                    1, 1, 1, 1, 2,
                    1, 1, 1, 1, 2,
                    2, 2, 2, 2, 2,
                },
                new[] {//size
                    5, 10
                },
                new [] {//entrance
                    -11, -1
                },
                new[] {//exits
                    3, 0, -5
                }
            }      //dungeon4
        }, //left  (3)
        new[,] {   //center(4)
            {         //dungeon1
                new[] {//blocks
                    2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                    1, 1, 1, 1, 5, 1, 5, 1, 1, 1, 1,
                    2, 2, 3, 1, 2, 1, 2, 1, 3, 2, 2,
                    0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0
                },
                new[] {//size
                    11, 6
                },
                new[] {//entrance
                    0, 0//[x, y]
                },
                new[] {//exits
                    1, 11, 1, 3, 0, 1//[facing, x, y]  facing: 0=up, 1=right, 2=down, 3=left, 4=center, 9=freespace
                }
            }       //dungeon1
        }  //center(4)
    };
    int[,] dungeon2spawnpos =//posisjon og dungeontype som skal spawnes next ltfdgg   
    {
        {
            9, 0, 0
        },
        {
            9, 0, 0
        },
        {
            9, 0, 0
        },
        {
            9, 0, 0
        },
        {
            9, 0, 0
        },
        {
            9, 0, 0
        },
        {
            9, 0, 0
        },
        {
            9, 0, 0
        },
        {
            9, 0, 0
        },
        {
            9, 0, 0
        },
        {
            9, 0, 0
        },
        {
            9, 0, 0
        },
        {
            9, 0, 0
        },
        {
            9, 0, 0
        },
        {
            9, 0, 0
        },
        {
            9, 0, 0
        }
    };


    // Start is called before the first frame update
    void Start()
    {
        Generation();
        SpawnDungeon();
        SpawnDungeon2();
    }

    void Generation() 
    {
        int stone_variation;
        float coal_ore_noise;
        float copper_ore_noise;
        float iron_ore_noise;
        for (int x = 0; x < map_width; x++) // SPAWNING TERRAIN
        {
            int height = Mathf.RoundToInt(heightgen * Mathf.PerlinNoise(x / smoothnessgen, seed) + heightgen / 5 * Mathf.PerlinNoise(x / smoothnessgen * 2, seed + 1000));
            stone_variation = 8 - 8 * height / Mathf.RoundToInt(heightgen);
            if (stone_variation < 0)
            {
                stone_variation = 0;
            }
            for (int y = 0; y < map_height + height; y++)
            {
                coal_ore_noise   = Mathf.PerlinNoise(x*coal_scale    , y * coal_scale);
                copper_ore_noise = Mathf.PerlinNoise(x * copper_scale, (y + seed * seed) * copper_scale);
                iron_ore_noise   = Mathf.PerlinNoise(x * iron_scale  , (y + seed * seed^2314234) * iron_scale); // 2026: make unused iron ore generate
                if (y < map_height + height / (Mathf.Pow(1.25f,layer_height) + 0.15) - (20 * layer_height))
                {
                    //Setblock(dense_stone, x, y);
                    StoneTilemap.SetTile(new Vector3Int(x, y, 0), dense_stone); 
                }
                else if (y < map_height + height - stone_variation)
                {
                    //Setblock(stone, x, y);
                    if (coal_ore_noise >= coal_size)
                    {
                        StoneTilemap.SetTile(new Vector3Int(x, y, 0), stone_coal_ore);
                    } else if (copper_ore_noise >= copper_size)
                    {
                        StoneTilemap.SetTile(new Vector3Int(x, y, 0), stone_copper_ore);
                    } else if (iron_ore_noise >= iron_size)
                    {
                        StoneTilemap.SetTile(new Vector3Int(x, y, 0), stone_iron_ore);
                    }
                    else
                    {
                        StoneTilemap.SetTile(new Vector3Int(x, y, 0), stone);
                    }
                }  
                else 
                {
                    //Setblock(dirt, x, y);
                    DirtTilemap.SetTile(new Vector3Int(x, y, 0), dirt);
                }
            }
            //Setblock(grass, x, height);
            DirtTilemap.SetTile(new Vector3Int(x, map_height + height, 0), grass);
        }
    }
    void SpawnDungeon()
    {
        float dungeon_noise;//where on the map a dungeon will spawn
        for (int x = 0; x < map_width; x++)//for each x
        {
            int height = Mathf.RoundToInt(heightgen * Mathf.PerlinNoise(x / smoothnessgen, seed) + heightgen / 5 * Mathf.PerlinNoise(x / smoothnessgen * 2, seed + 1000));//find out the height
            for (int y = 0; y < map_height + height; y++)//for each height
            {
                dungeon_noise = Mathf.PerlinNoise(x * dungeon_scale, y * dungeon_scale);//find out the dungeon noise
                if (dungeon_noise >= dungeon_size)//if it meet the requirments it will spawn a dungeon
                {
                    for (int loc_y = 0; loc_y < 7; loc_y++)//for each loc_y
                    {
                        for (int loc_x = 0; loc_x < 7; loc_x++)//for each loc_x
                        {
                            if (dungeon_components[loc_y * 7 + loc_x] == 1)//check if item loc_x + 7 for each loc_y is 1 then:
                            {
                                StoneTilemap.SetTile(new Vector3Int(x + loc_x, y - loc_y - 7, 0), brick);//spawn block at x + relative x pos (loc_x) and y + relative y pos(loc_y) - 7 to make it spawn 7 blocks lower
                            }
                            else if (dungeon_components[loc_y * 7 + loc_x] == 2)
                            {
                                StoneTilemap.SetTile(new Vector3Int(x + loc_x, y - loc_y - 7, 0), chest);
                            }
                            else
                            {
                                StoneTilemap.SetTile(new Vector3Int(x + loc_x, y - loc_y - 7, 0), air);
                            }
                        }
                    }
                }
            }
        }
    }
    void SpawnDungeon2()
    {
        float dungeon_noise;
        for (int x = 0; x < map_width; x++)
        {
            int height = Mathf.RoundToInt(heightgen * Mathf.PerlinNoise(x / smoothnessgen, seed) + heightgen / 5 * Mathf.PerlinNoise(x / smoothnessgen * 2, seed+ 1000));
            for (int y = 0; y < map_height + height; y++)
            {
                dungeon_noise = Mathf.PerlinNoise(x * dungeon2_scale, (y + seed) * dungeon2_scale);
                if (dungeon_noise >= dungeon2_size)
                {
                    int direction = 4;
                    int dungeon = 0;
                    int pointer_x = x;//where the next dungeon will spawn, when a new piece of dungeon will spawn it will retrieve coords from dungeon2spawnpos and spawn dungeon from there. when a new coord for new dungeon is generated it will take the pointer coords and add the next dungeon relative coords to it and add it to the list
                    int pointer_y = y;
                    while (true) //putt while true her
                    {//set values,   build structure,   store path outputs to list,   exit if list is empty,   set pointer to list item,   move pointer to starting pos of spawning,   changing dungeon
                        for (int loc_y = 0; loc_y < dungeon2[direction][dungeon, 1][1]; loc_y++)//spawning
                        {
                            for (int loc_x = 0; loc_x < dungeon2[direction][dungeon, 1][0]; loc_x++) //dungeon2[direction, 0, 1][0] = x lengde på structuren (11)
                            {
                                if (dungeon2[direction][dungeon, 0][loc_y * dungeon2[direction][dungeon, 1][0] + loc_x] == 1)
                                {
                                    StoneTilemap.SetTile(new Vector3Int(pointer_x + loc_x, pointer_y - loc_y, 0), air);
                                }
                                else if (dungeon2[direction][dungeon, 0][loc_y * dungeon2[direction][dungeon, 1][0] + loc_x] == 2)
                                {
                                    if (StoneTilemap.GetTile(new Vector3Int(pointer_x + loc_x, pointer_y - loc_y, 0)) != air)
                                    {
                                        StoneTilemap.SetTile(new Vector3Int(pointer_x + loc_x, pointer_y - loc_y, 0), brick);
                                    }
                                }
                                else if (dungeon2[direction][dungeon, 0][loc_y * dungeon2[direction][dungeon, 1][0] + loc_x] == 3)
                                {
                                    if (StoneTilemap.GetTile(new Vector3Int(pointer_x + loc_x, pointer_y - loc_y, 0)) != air)
                                    {
                                        StoneTilemap.SetTile(new Vector3Int(pointer_x + loc_x, pointer_y - loc_y, 0), brick_slab);
                                    }
                                }
                                else if (dungeon2[direction][dungeon, 0][loc_y * dungeon2[direction][dungeon, 1][0] + loc_x] == 4)
                                {
                                    if (StoneTilemap.GetTile(new Vector3Int(pointer_x + loc_x, pointer_y - loc_y, 0)) != air)
                                    {
                                        StoneTilemap.SetTile(new Vector3Int(pointer_x + loc_x, pointer_y - loc_y, 0), brick_slab2);
                                    }
                                }
                                else if (dungeon2[direction][dungeon, 0][loc_y * dungeon2[direction][dungeon, 1][0] + loc_x] == 5)
                                {
                                    StoneTilemap.SetTile(new Vector3Int(pointer_x + loc_x, pointer_y - loc_y, 0), chest);
                                }
                            }
                        }
                        int spawnpos_amount = 0;
                        for (int element = 0; element < 16; element++)//inserting new part positions from EXIT (16 = length of dungeonspawnpos)
                        {
                            if (dungeon2spawnpos[element, 0] == 9)// dont store the positions as local, but insteasd global
                            {
                                dungeon2spawnpos[element, 0] = dungeon2[direction][dungeon, 3][spawnpos_amount * 3 + 0];
                                dungeon2spawnpos[element, 1] = dungeon2[direction][dungeon, 3][spawnpos_amount * 3 + 1] + pointer_x;
                                dungeon2spawnpos[element, 2] = dungeon2[direction][dungeon, 3][spawnpos_amount * 3 + 2] + pointer_y;
                                spawnpos_amount++;
                                if (spawnpos_amount == dungeon2[direction][dungeon, 3].Length / 3)
                                {
                                    break;
                                }
                            }
                        }
                        int used_spaces = 0;
                        for (int element = 0; element < 16; element++)
                        {
                            if (dungeon2spawnpos[element, 0] != 9)
                            {
                                used_spaces ++;
                            }
                        }
                        if (used_spaces == 0) //if there is not any more dungeon parts to be generated (so if dungeon is finsihed)
                        {
                            break;
                        }
                        for (int element = 0; element < 16; element++) //Update pointer variables (extract the quicc values from dungeon2spawnpoint)
                        {
                            if (dungeon2spawnpos[element, 0] != 9)
                            {
                                direction = dungeon2spawnpos[element, 0];
                                pointer_x = dungeon2spawnpos[element, 1];
                                pointer_y = dungeon2spawnpos[element, 2];
                                //reset dungeonspawnpos i posisjon <element>
                                dungeon2spawnpos[element, 0] = 9;
                                dungeon2spawnpos[element, 1] = 0;
                                dungeon2spawnpos[element, 2] = 0;
                                break;
                            }
                        }
                        if (direction == 1)
                        {
                            dungeon = Random.Range(0, 5);
                            if (dungeon == 4)
                            {
                                dungeon = 1;
                            }
                        }
                        else if (direction == 3)
                        {
                            dungeon = Random.Range(0, 3);
                        }
                        pointer_x += dungeon2[direction][dungeon, 2][0];
                        pointer_y += dungeon2[direction][dungeon, 2][1];
                    }
                }
            }
        }
    }

    /*void Setblock(GameObject obj, int width, int height) 
    {
        obj = Instantiate(obj, new Vector2(width, height), Quaternion.identity);
        obj.transform.parent = this.transform;
    }*/
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StoneTilemap.ClearAllTiles();
            DirtTilemap.ClearAllTiles();
            Generation();
            SpawnDungeon();
            SpawnDungeon2();
        }
    }
    /*IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();
        StoneTilemap.ClearAllTiles();
        DirtTilemap.ClearAllTiles();
    }*/
}
