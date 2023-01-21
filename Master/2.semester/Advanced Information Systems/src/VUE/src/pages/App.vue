<template>
  <div id="app">
    <NavbarFinal></NavbarFinal>
    <div>
      <MainSection name="Books" fullPage="/books/" :data="books" root="/books/"></MainSection>
      <!-- <MainSection name="E-Books"></MainSection> -->
      <MainSection name="Magazines" fullPage="/magazines/" :data="magazines" root="/magazines/"></MainSection>
      <AuthorSection v-if="authors.length"
                     name="Authors"
                     :data="authors"
                     root="/authors/"
                     fullPage="/authors/">

      </AuthorSection>
    </div>
  </div>
</template>

<script>

import MainSection from "@/components/main_page/MainSection";
import ApiConnect from "@/services/ApiConnect";
import AuthorSection from "@/components/genre_page/AuthorSection";
import NavbarFinal from "@/components/main_page/NavbarFinal";

export default {
  name: 'App',
  components: {
    MainSection,
    AuthorSection,
    NavbarFinal
  },
  data(){
    return {
      books: [],
      magazines: [],
      authors: [],
    }
  },
  methods: {
    getBooks(){
      ApiConnect.get('books').then((response) =>
          this.books = response.data,
      )},
    getMagazines(){
      ApiConnect.get('magazines/').then((response) =>
          this.magazines = response.data,
      )},
    getAuthors(){
      ApiConnect.get('authors/').then((response) => this.authors = response.data, )
    },

  },
  created() {
    this.getBooks();
    this.getMagazines();
    this.getAuthors();
  }
}
</script>

<style>
  @import "../assets/styles/main.css";
</style>
