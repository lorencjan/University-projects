<template>
  <div id="Genre">
    <NavbarFinal></NavbarFinal>
    <div>
      <h1 style="text-align:center">{{this.genre_name.name}}</h1>
    </div>
    <MainSection v-if="books.length" name="Books" :fullPage="'/books/?genres='+this.genre_name.name" :data="books" root="/books/"></MainSection>
    <AuthorSection v-if="authors.length"
                   name="Authors"
                   :data="authors"
                   root="/authors/"
                   :fullPage="fullPage">

    </AuthorSection>
    <h3 style="text-align: center" v-if="! books.length && ! authors.length">No books and authors for this genre.</h3>
  </div>
</template>

<script>

import MainSection from "@/components/main_page/MainSection";
import ApiConnect from "@/services/ApiConnect";
import AuthorSection from "@/components/genre_page/AuthorSection";
import NavbarFinal from "@/components/main_page/NavbarFinal";

export default {
  name: "Genre",
  components: {
    AuthorSection,
    MainSection,
    NavbarFinal
  },

  data(){
    return {
      books: [],
      genre_name: [],
      authors: [],
      fullPage : '/genre_authors/' + this.$route.params.id
    }
  },

  methods: {
    getBooks(name){
      let params = {params:{"genres": name}};
      ApiConnect.get('books/',params).then((response) =>
      {
          this.books = response.data;
          this.books.forEach(book => this.authors.push(book.authors[0]))
      }
      )},

    getName(){
      let id = this.$route.params.id
      if (typeof(this.$route.params.id) == 'undefined'){
        id = ''
      }
      ApiConnect.get('genres/'+id).then((response) =>
      {
        this.genre_name = response.data;
        this.getBooks(this.genre_name.name);
      }
      )},


  },
  created() {
    this.getName();
  },

}

</script>

<style scoped>
@import "../assets/styles/main.css";
</style>