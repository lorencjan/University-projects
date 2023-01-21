<template>
  <div id="author">
    <NavBarFinal></NavBarFinal>
    <b-container>

      <AuthorBigTile
        :name="author.name"
        :surname="author.surname"
        :birth="author.dateOfBirth ? new Date(author.dateOfBirth): null"
        :death="dateDeath"
        :description="author.description"
        :img = "author.photographPath"
      ></AuthorBigTile>

      <b-row v-if="books.length > 0">
        <b-col><h1>Books</h1></b-col>
      </b-row>
      <b-row v-if="books.length > 0">
        <MainTile v-for="load in books"
                  :img="load.coverPhotoPath"
                  :genres = load.genres
                  :type = load.language
                  :name = load.name
                  :authors = load.authors
                  :id = load.id
                  root = '/books/'
        >
        </MainTile>
      </b-row>
      <b-row v-if="magazines.length > 0">
        <b-col><h1>Magazines</h1></b-col>
      </b-row>
      <b-row v-if="magazines.length > 0">
        <MainTile v-for="load in magazines"
                  :img="load.coverPhotoPath"
                  :fields = load.fields
                  :type = load.language
                  :name = load.name
                  :authors = load.authors
                  :id = load.id
                  root = '/magazines/'
        >
        </MainTile>
      </b-row>
    </b-container>
  </div>
</template>

<script>
import ApiConnect from "@/services/ApiConnect";
import MainTile from "@/components/main_page/MainTile";
import AuthorBigTile from "@/components/author/AuthorBigTile";
import NavBarFinal from "@/components/main_page/NavbarFinal";

export default {
  name: "Author",
  components: {
    MainTile,
    AuthorBigTile,
    NavBarFinal
  },
  data(){
    return {
      author: {},
      books: [],
      magazines: [],
    }
  },
  methods: {
    async getAuthorsBooks() {
      const resp = await ApiConnect.get('authors/' + this.$route.params.id);
      this.author = resp.data;
      ApiConnect.get('books/', {params: {author: this.author.name}}).then((response) =>
          this.books = response.data
      )
      ApiConnect.get('magazines/', {params: {author: this.author.name}}).then((response) =>
          this.magazines = response.data
      )
    },
  },
  created() {
    this.getAuthorsBooks(this.author.name);
  },
   computed:{
    dateDeath (){
      if (this.author.dateOfDeath === null){
        return undefined;
      }
      else {
        return new Date(this.author.dateOfDeath)
      }
    }
   }
}
</script>

<style scoped>

</style>