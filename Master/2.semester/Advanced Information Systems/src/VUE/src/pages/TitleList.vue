<template>
  <div id="titles">
    <NavbarFinal></NavbarFinal>
        <b-tabs content-class="mt-3" fill class="bg-light">
          <b-tab title="Books" active>
            <dataTable
                endpointGet="/books/"
                endpointEdit="/edit_books/"
                endpointDel="/books/"
                type="book"
                :fields="fieldsBooks"
                sortBy="isbn"
                :parse="parseBooks"
                tableId="tableBooks"
            >
            </dataTable>
          </b-tab>
          <b-tab title="Magazines">
            <dataTable
                endpointGet="/magazines/"
                endpointEdit="/edit_magazines/"
                endpointDel="/magazines/"
                type="magazine"
                :fields="fieldsMagazines"
                sortBy="issn"
                :parse="parseMagazines"
                tableId="tableMagazines"
            >
            </dataTable>
          </b-tab>
          <b-tab title="Fields">
            <dataTable
                endpointGet="/fields/"
                endpointEdit="/edit_fields/"
                endpointDel="/fields/"
                type="field"
                :fields="fieldsFields"
                sortBy="name"
                :parse="parseFields"
                tableId="tableFields"
            >
            </dataTable>
          </b-tab>
          <b-tab title="Genres">
            <dataTable
                endpointGet="/genres/"
                endpointEdit="/edit_genres/"
                endpointDel="/genres/"
                type="genre"
                :fields="fieldsGenres"
                sortBy="name"
                :parse="parseGenres"
                tableId="tableGenres"
            >
            </dataTable>
          </b-tab>
        </b-tabs>
  </div>
</template>

<script>
import dataTable from "@/components/title_list/dataTable";
import ApiConnect from "@/services/ApiConnect";
import NavbarFinal from "@/components/main_page/NavbarFinal";

function getNames(data) {
  let string = "";
  data.forEach((dato) => string += "&" + dato.name);
  string = string.replace('&','')
  string = string.replaceAll("&",", ")
  return string;
}

function getAuthors(data) {
  let string = "";
  data.forEach((dato) => string += "&" + dato.surname + " " + dato.name);
  string = string.replace('&','')
  string = string.replaceAll("&",", ")
  return string;
}

Array.prototype.getUnique = function() {
  var o = {}, a = []
  for (var i = 0; i < this.length; i++) o[this[i]] = 1
  for (var e in o) a.push(e)
  return a
}

export default {
  name: "TitleList",
  components: {
    dataTable,
    NavbarFinal
  },
  data() {
    return{
      fieldsBooks: [
        {key: 'isbn', sortable: true},
        {key: 'name', sortable: true},
        {key: 'publisher', sortable: true},
        {key: 'authors', sortable: true},
        {key: 'genres', sortable: true},
        {key: 'edit', sortable: false},
        {key: 'delete', sortable: false},

      ],
      fieldsMagazines: [
        {key: 'issn', sortable: true},
        {key: 'name', sortable: true},
        {key: 'publisher', sortable: true},
        {key: 'authors', sortable: true},
        {key: 'fields', sortable: true},
        {key: 'edit', sortable: false},
        {key: 'delete', sortable: false},
      ],
      fieldsFields: [
        {key: 'name', sortable: true},
        {key: 'authorCount', sortable: true},
        {key: 'magazinesCount', sortable: true},
        {key: 'edit', sortable: false},
        {key: 'delete', sortable: false},
      ],
      fieldsGenres: [
        {key: 'name', sortable: true},
        {key: 'authorCount', sortable: true},
        {key: 'magazinesCount', sortable: true},
        {key: 'edit', sortable: false},
        {key: 'delete', sortable: false},
      ]

    }
  },
  methods: {
    parseBooks(data){
      data.forEach(function (book){
        book.genres = getNames(book.genres);
        book.authors = getAuthors(book.authors);
      })
      return data
    },
    parseMagazines(data){
      data.forEach(function (book){
        book.fields = getNames(book.fields);
        book.authors = getAuthors(book.authors);
      })
      return data
    },
    parseFields(data,oldData){
      if(oldData === undefined){
        data.forEach(function (book){
          book.authorCount = 0
          book.magazinesCount = 0
        })
        data.forEach(subparse)
      }else{
        data.forEach(function (book, index){
          book.authorCount = oldData[index].authorCount
          book.magazinesCount = oldData[index].magazinesCount
        })
      }

      function subparse(field){
        let paramsMagazine = {params: {"fields": field.name}};
        ApiConnect.get('/magazines/', paramsMagazine).then(resp =>{
          field.magazinesCount = resp.data.length
          let authors = []
          resp.data.forEach( magazine =>{
            magazine.authors.forEach(author => {
              authors.push(author.name+author.surname)
            })
          })
          field.authorCount = authors.getUnique().length
        })
      }

      return data;
    },
    parseGenres(data,oldData){
      if(oldData === undefined){
        data.forEach(function (book){
          book.authorCount = 0
          book.magazinesCount = 0
        })
        data.forEach(subparse)
      }else{
        data.forEach(function (book, index){
          book.authorCount = oldData[index].authorCount
          book.magazinesCount = oldData[index].magazinesCount
        })
      }

      function subparse(field){
        let paramsMagazine = {params: {"genres": field.name}};
        ApiConnect.get('/books/', paramsMagazine).then(resp =>{
          field.magazinesCount = resp.data.length
          let authors = []
          resp.data.forEach( magazine =>{
            magazine.authors.forEach(author => {
              authors.push(author.name+author.surname)
            })
          })
          field.authorCount = authors.getUnique().length
        })
      }

      return data;
    }
  }
}
</script>

<style scoped>

</style>