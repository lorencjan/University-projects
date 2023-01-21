/***********************************************************/
/**      Projekt 3 - Jednoducha shlukova analyza          **/
/**                    Jan Lorenc                         **/
/**                 login: xloren15 					  **/
/**						4.12.2018                         **/
/** JEDNA SE POUZE O PUVODNI KOSTRU S DOPLNENYMI OBLASTMI **/
/** "TODO" A TOUTO ZMENENOU HLAVICKOU - ZBYTEK NEDOTCEN   **/
/***********************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include <math.h> // sqrtf
#include <limits.h> // INT_MAX

/*****************************************************************
 * Ladici makra. Vypnout jejich efekt lze definici makra
 * NDEBUG, napr.:
 *   a) pri prekladu argumentem prekladaci -DNDEBUG
 *   b) v souboru (na radek pred #include <assert.h>
 *      #define NDEBUG
 */
#ifdef NDEBUG
#define debug(s)
#define dfmt(s, ...)
#define dint(i)
#define dfloat(f)
#else

// vypise ladici retezec
#define debug(s) printf("- %s\n", s)

// vypise formatovany ladici vystup - pouziti podobne jako printf
#define dfmt(s, ...) printf(" - "__FILE__":%u: "s"\n",__LINE__,__VA_ARGS__)

// vypise ladici informaci o promenne - pouziti dint(identifikator_promenne)
#define dint(i) printf(" - " __FILE__ ":%u: " #i " = %d\n", __LINE__, i)

// vypise ladici informaci o promenne typu float - pouziti
// dfloat(identifikator_promenne)
#define dfloat(f) printf(" - " __FILE__ ":%u: " #f " = %g\n", __LINE__, f)

#endif

/*****************************************************************
 * Deklarace potrebnych datovych typu:
 *
 * TYTO DEKLARACE NEMENTE
 *
 *   struct obj_t - struktura objektu: identifikator a souradnice
 *   struct cluster_t - shluk objektu:
 *      pocet objektu ve shluku,
 *      kapacita shluku (pocet objektu, pro ktere je rezervovano
 *          misto v poli),
 *      ukazatel na pole shluku.
 */

struct obj_t {
    int id;
    float x;
    float y;
};

struct cluster_t {
    int size;
    int capacity;
    struct obj_t *obj;
};
/**moje - definice konstant **/
#define MIN_DISTANCE 1500 //minimalni vzdalenost mezi clustery
/*****************************/


/*****************************************************************
 * Deklarace potrebnych funkci.
 *
 * PROTOTYPY FUNKCI NEMENTE
 *
 * IMPLEMENTUJTE POUZE FUNKCE NA MISTECH OZNACENYCH 'TODO'
 *
 */

/*
 Inicializace shluku 'c'. Alokuje pamet pro cap objektu (kapacitu).
 Ukazatel NULL u pole objektu znamena kapacitu 0.
*/
void init_cluster(struct cluster_t *c, int cap)
{
    assert(c != NULL);
    assert(cap >= 0);

    // TODO
    /**********************************************************************/
    c->capacity = cap;				//nastavi kapacitu na hodnotu z parametru
    c->size = 0;					//nastavi velikost na 0
    if(cap == 0)					//pokud je nulova kapacita																																		
    	c->obj = NULL;				//pole bude NULL
    else							//jinak se alokuje dle kapacity
	{
		c->obj = malloc(cap*sizeof(struct obj_t));
		if(c->obj == NULL)
			fprintf(stderr, "Failed to allocate memory for a new cluster!\n");
	}	
    /**********************************************************************/
}

/*
 Odstraneni vsech objektu shluku a inicializace na prazdny shluk.
 */
void clear_cluster(struct cluster_t *c)
{
    // TODO
    /*********************************************************************/
    if(c->capacity)     //jestli cluster neni jiz prazdny
    	free(c->obj);   //tak uvolni pamet pole objektu
    c->obj = NULL;      //a vynuluj vse ostatni
    c->capacity = c->size = 0;
    /*********************************************************************/
}

/// Chunk of cluster objects. Value recommended for reallocation.
const int CLUSTER_CHUNK = 10;
/*
 Zmena kapacity shluku 'c' na kapacitu 'new_cap'.
 */
struct cluster_t *resize_cluster(struct cluster_t *c, int new_cap)
{
    // TUTO FUNKCI NEMENTE
    assert(c);
    assert(c->capacity >= 0);
    assert(new_cap >= 0);

    if (c->capacity >= new_cap)
        return c;

    size_t size = sizeof(struct obj_t) * new_cap;

    void *arr = realloc(c->obj, size);																									
    if (arr == NULL)
        return NULL;

    c->obj = (struct obj_t*)arr;
    c->capacity = new_cap;
    return c;
}

/*
 Prida objekt 'obj' na konec shluku 'c'. Rozsiri shluk, pokud se do nej objekt
 nevejde.
 */
void append_cluster(struct cluster_t *c, struct obj_t obj)
{
    // TODO
    /**************************************************/
    if(c->size == c->capacity)                                 //jestli velikost dosahla kapacity
        c = resize_cluster(c, c->capacity+CLUSTER_CHUNK);	   //zvetsi kapacitu
    if(c->size < c->capacity && c!=NULL)
    {
        c->obj[c->size] = obj;                                 //prida na konec pole novy objekt
        c->size++;                                             //zvetsi velikost
    }
    /**************************************************/
}

/*
 Seradi objekty ve shluku 'c' vzestupne podle jejich identifikacniho cisla.
 */
void sort_cluster(struct cluster_t *c);

/*
 Do shluku 'c1' prida objekty 'c2'. Shluk 'c1' bude v pripade nutnosti rozsiren.
 Objekty ve shluku 'c1' budou serazeny vzestupne podle identifikacniho cisla.
 Shluk 'c2' bude nezmenen.
 */
void merge_clusters(struct cluster_t *c1, struct cluster_t *c2)
{
    assert(c1 != NULL);
    assert(c2 != NULL);

    // TODO
    /**************************************************/
    for(int i=0; i<c2->size; i++)           //vsechny objekty jednoho clusteru da do druheho
        append_cluster(c1, c2->obj[i]);
    sort_cluster(c1);                       //usporada je vzestupne
    /**************************************************/
}

/**********************************************************************/
/* Prace s polem shluku */

/*
 Odstrani shluk z pole shluku 'carr'. Pole shluku obsahuje 'narr' polozek
 (shluku). Shluk pro odstraneni se nachazi na indexu 'idx'. Funkce vraci novy
 pocet shluku v poli.
*/
int remove_cluster(struct cluster_t *carr, int narr, int idx)
{
    assert(idx < narr);
    assert(narr > 0);

    // TODO
    /**********************************************************************/ 
    clear_cluster(&carr[idx]);		  //vycisteni clusteru       
    for(int i=idx; i<narr; i++)       
        carr[i] = carr[i+1];		  //presunuti pole o jednu polozku
    return narr-1;
    /**********************************************************************/
}

/*
 Pocita Euklidovskou vzdalenost mezi dvema objekty.
 */
float obj_distance(struct obj_t *o1, struct obj_t *o2)
{
    assert(o1 != NULL);
    assert(o2 != NULL);

    // TODO
    /******************************************************************************/
    return sqrt( (o1->x-o2->x)*(o1->x-o2->x) + (o1->y-o2->y)*(o1->y-o2->y) );
    /******************************************************************************/
}

/*
 Pocita vzdalenost dvou shluku.
*/
float cluster_distance(struct cluster_t *c1, struct cluster_t *c2)
{
    assert(c1 != NULL);
    assert(c1->size > 0);
    assert(c2 != NULL);
    assert(c2->size > 0);

    // TODO
    /******************************************************************************/
    float distance, minDistance = MIN_DISTANCE; 
    for(int i = 0; i < c1->size; i++)
        for(int j = 0; j < c2->size; j++)
        {
            distance = obj_distance(&c1->obj[i], &c2->obj[j]);      //zjisti vzdalenost clusteru
            if(distance < minDistance)                              //jestli je vzdalenost mensi nez dosavadni nejmensi
               minDistance = distance;                              //prepise ji
        }
    return minDistance;
    /******************************************************************************/
}

/*
 Funkce najde dva nejblizsi shluky. V poli shluku 'carr' o velikosti 'narr'
 hleda dva nejblizsi shluky. Nalezene shluky identifikuje jejich indexy v poli
 'carr'. Funkce nalezene shluky (indexy do pole 'carr') uklada do pameti na
 adresu 'c1' resp. 'c2'.
*/
void find_neighbours(struct cluster_t *carr, int narr, int *c1, int *c2)
{
    assert(narr > 0);

    // TODO
    /******************************************************************************/
    float distance, minDistance = MIN_DISTANCE; 
    for(int i=0; i < narr; i++)
        for(int j=i+1; j < narr; j++)
        {
            distance = cluster_distance(&carr[i], &carr[j]); //zjisti vzdalenost clusteru
            //pokud je vzdalenost mensi nez dosavadni nejmensi, premise hodnoty
            if(distance < minDistance)
            {
                *c1 = i;
                *c2 = j;
                minDistance = distance;
            }
        }
    /******************************************************************************/
}

// pomocna funkce pro razeni shluku
static int obj_sort_compar(const void *a, const void *b)
{
    // TUTO FUNKCI NEMENTE
    const struct obj_t *o1 = (const struct obj_t *)a;
    const struct obj_t *o2 = (const struct obj_t *)b;
    if (o1->id < o2->id) return -1;
    if (o1->id > o2->id) return 1;
    return 0;
}

/*
 Razeni objektu ve shluku vzestupne podle jejich identifikatoru.
*/
void sort_cluster(struct cluster_t *c)
{
    // TUTO FUNKCI NEMENTE
    qsort(c->obj, c->size, sizeof(struct obj_t), &obj_sort_compar);
}

/*
 Tisk shluku 'c' na stdout.
*/
void print_cluster(struct cluster_t *c)
{
    // TUTO FUNKCI NEMENTE
    for (int i = 0; i < c->size; i++)
    {
        if (i) putchar(' ');
        printf("%d[%g,%g]", c->obj[i].id, c->obj[i].x, c->obj[i].y);
    }
    putchar('\n');
}

/*
 Ze souboru 'filename' nacte objekty. Pro kazdy objekt vytvori shluk a ulozi
 jej do pole shluku. Alokuje prostor pro pole vsech shluku a ukazatel na prvni
 polozku pole (ukalazatel na prvni shluk v alokovanem poli) ulozi do pameti,
 kam se odkazuje parametr 'arr'. Funkce vraci pocet nactenych objektu (shluku).
 V pripade nejake chyby uklada do pameti, kam se odkazuje 'arr', hodnotu NULL.
*/
int load_clusters(char *filename, struct cluster_t **arr)
{
    assert(arr != NULL);

    // TODO
    /*********************************************************/
    //otevreme a zkotrolujeme soubor z argumentu
    FILE * file = fopen(filename, "r");
    if(!file)
    {
        fprintf(stderr, "File couldn't be opened! Enter correct name of the file in program arguments\n");
        return -1;
    }
    //definice promennych
    int num_of_objects, num_of_clusters = 0, cluster_init_cap = 0;
    if(fscanf(file, "count=%d", &num_of_objects) != 1 || num_of_objects < 1)       //nacteni poctu objektu z prvniho radku
	 {
		  fprintf(stderr, "No clusters were loaded!\n");
		  fclose(file);
		  return -1;
	 }    
    //alokace pole clusteru
    struct cluster_t *cluster_array = malloc((num_of_objects+1)*sizeof(struct cluster_t)); 
    if(cluster_array == NULL)
    {
        fprintf(stderr, "Failed to allocate memory for array of clusters!\n");
		fclose(file);        
        return -1;
    }
    *arr = cluster_array;      //prirazeni prvniho prvku pole ukazateli arr
    
    //plneni pole clusteru
    for(int i=0; i < num_of_objects; i++)
    {
        struct obj_t obj;                                                           //definice objektu na radku
        int num_of_object_arg = fscanf(file, "%d %f %f", &obj.id, &obj.x, &obj.y);  //nacteni hodnot z radku
        //pokud se nenacetli vsechny potrebne hodnoty, vypise chybu a ukonci program
        if(num_of_object_arg == -1)													//jestli se nic nenacetlo jedna se
        	continue;																//o prazdny radek -> preskoci se
        if(num_of_object_arg != 3)
        {
            fprintf(stderr, "An object doesn't have all its arguments!\n");
            goto dealloc;
        }
        //pokud se u nejakeho bodu presahne rozsah, vypise chybu a ukonci program
        if(obj.x > 1000 || obj.x < 0 ||obj.y > 1000 || obj.y < 0)
        {
            fprintf(stderr, "An object has values out of range!\n");
            goto dealloc;
        }
        //inicialize clusteru a pridani objektu do nej
        init_cluster(&cluster_array[i], cluster_init_cap);
        append_cluster(&cluster_array[i], obj);
        num_of_clusters++;
    }
    if(fclose(file) != 0)
       fprintf(stderr, "Error - file %s could'n be closed! Program continues to run anyway!", filename);
     
    return num_of_clusters;

    dealloc:
    fclose(file);
    for(int i = 0; i<num_of_clusters; i++)
    	clear_cluster(&cluster_array[i]);
    return 0;
    /*********************************************************/
}

/*
 Tisk pole shluku. Parametr 'carr' je ukazatel na prvni polozku (shluk).
 Tiskne se prvnich 'narr' shluku.
*/
void print_clusters(struct cluster_t *carr, int narr)
{
    printf("Clusters:\n");
    for (int i = 0; i < narr; i++)
    {
        printf("cluster %d: ", i);
        print_cluster(&carr[i]);
    }
}

int main(int argc, char *argv[])
{
    struct cluster_t *clusters;

    // TODO
    int num_of_clusters = (argc > 2) ? atoi(argv[2]) : 1;           //nacteni poctu chtenych clusteru z argumentu
    if(num_of_clusters < 1)
    {
    	fprintf(stderr, "There must be positive number of clusters!\n");
    	return 0;
    }
    int num_of_loaded_clusters = load_clusters(argv[1], &clusters); //nacteni clusteru a ulozeni jejich poctu do promenne
	if(num_of_loaded_clusters == -1) 								//konci program, pokud doslo k chybe mimo alokaci	
	    return 0;   												//a neni tedy treba dealokovat - zbytecne volat free()
    if(num_of_loaded_clusters == 0)                                 //konci program, jestli se nic nenacetlo / chyba u alokace
    {    goto dealloc;}                                             //chybova hlaska se nachazi v funkci
	if(num_of_clusters > num_of_loaded_clusters)					//jestli chci udelat vice clusteru, nez se jich vubec nacetlo	
	{																//vypis chybu a ukonci program	
	    fprintf(stderr, "Program cannot make %d out of %d clusters!\n", num_of_clusters, num_of_loaded_clusters);
	    goto dealloc;	
	}	                                              

    //dokud neni spojenych clusteru stejne jako pozadovanych, vyhleda sousedy, spoji je a oddela druheho
    int neighbour1_index, neighbour2_index;                         //id sousedicich clusteru
    while(num_of_loaded_clusters != num_of_clusters)
    {
        find_neighbours(clusters, num_of_loaded_clusters, &neighbour1_index, &neighbour2_index);
        merge_clusters(&clusters[neighbour1_index], &clusters[neighbour2_index]);
        num_of_loaded_clusters = remove_cluster(clusters, num_of_loaded_clusters, neighbour2_index);
    }

    //vypsani vyslednych clusteru
    print_clusters(clusters, num_of_clusters);

    //dealokace pameti
	dealloc:
    for(int i=0; i<num_of_loaded_clusters; i++)
        clear_cluster(&clusters[i]);          //vycisteni jednotlivych clusteru (dealokace pole objektu)   
    free(clusters);   // clusters sice nebyl alokovan dynamicky, ukazuje vsak na dynamicky alokovanou pamet      
    return 0;
}