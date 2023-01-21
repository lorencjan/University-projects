<template>
  <div id="readers">
    <NavbarFinal></NavbarFinal>
    <b-tabs content-class="mt-3" fill class="bg-light">
      <b-tab title="Readers" active>
        <data-table
            endpointGet="/readers/"
            endpointEdit="/readers/"
            endpointDel="/readers/"
            endpoint-create="/createReader/"
            type="readers"
            :fields="fieldsReaders"
            sortBy="fullname"
            :parse="parseReaders"
            tableId="tableReaders"
        >
        </data-table>
      </b-tab>
    </b-tabs>
  </div>
</template>

<script>
import dataTable from "@/components/title_list/dataTable";
import NavbarFinal from "@/components/main_page/NavbarFinal";

export default {
  name: "ReadersList",

  components: {
    dataTable,
    NavbarFinal
  },

  data() {
    return{
      fieldsReaders: [
        {key: 'fullname', sortable: true},
        {key: 'email', sortable: true},
        {key: 'role', sortable: true},
        {key: 'address', sortable: true},
        {key: 'edit', sortable: false},
        {key: 'delete', sortable: false},

      ],
    }
  },
  methods: {
    parseReaders(data){
      data.forEach(reader => {
        reader.address = reader.city + ', ' +
                         reader.street + ' ' +
                         reader.houseNumber + ', ' +
                         reader.postcode
      })
      return data
    },

  },
}
</script>

<style scoped>

</style>