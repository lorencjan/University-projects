<template>
  <div id="authors">
    <NavbarFinal></NavbarFinal>
    <b-tabs content-class="mt-3" fill class="bg-light">
      <b-tab title="Authors" active>
        <data-table
            endpointGet="/authors/"
            endpointEdit="/edit_authors/"
            endpointDel="/authors/"
            type="authors"
            :fields="fieldsAuthors"
            sortBy="name"
            :parse="parseAuthors"
            tableId="tableAuthors"
        >
        </data-table>
      </b-tab>
    </b-tabs>
  </div>
</template>

<script>
import DataTable from "@/components/title_list/dataTable";
import Vue from "vue";
import NavbarFinal from "@/components/main_page/NavbarFinal";

export default {
  name: "AuthorsList",
  components: {
    DataTable,
    NavbarFinal
  },

  data() {
    return{
      fieldsAuthors: [
        {key: 'name', sortable: true},
        {key: 'dateOfBirth', sortable: true},
        {key: 'dateOfDeath', sortable: true},
        {key: 'edit', sortable: false},
        {key: 'delete', sortable: false},
      ],
    }
  },

  methods: {
    parseAuthors(data){
      data.forEach(author => {
        author.name = author.name + ' ' + author.surname;
        author.dateOfBirth = Vue.filter('formatDate')(new Date(author.dateOfBirth))
        author.dateOfDeath = Vue.filter('formatDate')(new Date(author.dateOfDeath))
      })
      return data
    },

  },
}
</script>

<style scoped>

</style>