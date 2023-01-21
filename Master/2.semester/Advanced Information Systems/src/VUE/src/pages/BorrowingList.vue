<template>
  <div id="borrowings">
    <NavbarFinal></NavbarFinal>
    <b-tabs content-class="mt-3" fill class="bg-light">
      <b-tab title="Hard Copy Borrowings" active>
        <data-table
            endpointGet="/hard-copy-borrowings/"
            endpointEdit="/edit_hard-copy-borrowings/"
            endpointDel="/hard-copy-borrowings/"
            type="hard-copy-borrowings"
            :fields="fieldsBorrowings"
            sortBy="dateOfBorrowStart"
            :parse="parseHardCopyBorrowings"
            tableId="tableHardBorrowings"
        >
        </data-table>
      </b-tab>
      <b-tab title="ElectronicCopyBorrowings">
        <data-table
            endpointGet="/electronic-copy-borrowings/"
            endpointDel="/electronic-copy-borrowings/"
            :endpointCreate="null"
            type="electronic-copy-borrowings"
            :fields="fieldsBorrowings2"
            sortBy="dateOfBorrowStart"
            :parse="parseElectronicCopyBorrowings"
            tableId="tableElectronicBorrowings"
        >
        </data-table>
      </b-tab>
    </b-tabs>
  </div>
</template>

<script>
import DataTable from "@/components/title_list/dataTable";
import Vue from "vue";
import NavbarFinal
  from "@/components/main_page/NavbarFinal";
export default {
  name: "BorrowingList",
  components: {
    DataTable,
    NavbarFinal
  },
  data() {
    return{
      fieldsBorrowings: [
        {key: 'dateOfBorrowStart', sortable: true},
        {key: 'dateOfBorrowEnd', sortable: true},
        {key: 'borrowCounter', sortable: true},
        {key: 'state', sortable: true},
        {key: 'returnDate', sortable: true},
        {key: 'reader_name', sortable: true},
        {key: 'title', sortable: true},
        {key: 'edit', sortable: false},
        {key: 'delete', sortable: false},
      ],
      fieldsBorrowings2: [
        {key: 'dateOfBorrowStart', sortable: true},
        {key: 'dateOfBorrowEnd', sortable: true},
        {key: 'borrowCounter', sortable: true},
        {key: 'reader_name', sortable: true},
        {key: 'title', sortable: true},
        {key: 'delete', sortable: false},
      ],
    }
  },
  methods: {
    parseHardCopyBorrowings(data){
      data.forEach(function (borrowing){
        borrowing.reader_name = borrowing.reader.fullname
        borrowing.title = borrowing.exemplar.titleName;
        borrowing.dateOfBorrowStart = Vue.filter('formatDate')(new Date(borrowing.dateOfBorrowStart))
        borrowing.dateOfBorrowEnd = Vue.filter('formatDate')(new Date(borrowing.dateOfBorrowEnd))
        if(borrowing.returnDate === null) borrowing.returnDate = ''
        else borrowing.returnDate = Vue.filter('formatDate')(new Date(borrowing.returnDate))
      })
      return data;
    },
    parseElectronicCopyBorrowings(data){
      data.forEach(function (borrowing){
        borrowing.reader_name = borrowing.reader.fullname
        borrowing.title = borrowing.electronicCopy.titleName;
        borrowing.dateOfBorrowStart = Vue.filter('formatDate')(new Date(borrowing.dateOfBorrowStart))
        borrowing.dateOfBorrowEnd = Vue.filter('formatDate')(new Date(borrowing.dateOfBorrowEnd))
      })
      return data;
    },
    makeToast(type) {
      this.$bvToast.toast(type+' has been deleted successfully.', {
        title: 'Library',
        variant: 'success',
        autoHideDelay: 5000,
      })
    },
  },

}
</script>

<style scoped>

</style>