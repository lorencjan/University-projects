<template>
  <b-container>
    <b-row>
      <b-col cols="11">
        <b-pagination
            v-model="currentPageBook"
            :total-rows="rows"
            :per-page="perPageBook"
            :aria-controls="tableId"
            align="center"
        ></b-pagination>
      </b-col>
      <b-col class="text-right" cols="1" v-if="endpointCreate !== null">
        <router-link :to="{path: endpointCreateComp}">
          <b-button variant="success">create</b-button>
        </router-link>
      </b-col>
    </b-row>
    <b-table
        :id="tableId"
        :ref="tableId"
        :per-page="perPageBook"
        :current-page="currentPageBook"
        :busy.sync="isBusy"
        :items="myProvider"
        :fields="fields"
        :sort-by.sync="sortByVal"
        :sort-desc.sync="sortDesc"
        striped hover
    >
      <template v-slot:cell(edit)="{ item }" v-if="endpointEdit !== undefined">
        <router-link :to="{path: endpointEdit+item.id}">
          <font-awesome-icon icon="fa-solid fa-pen-to-square" size="2x" type="button"/>
        </router-link>
      </template>
      <template v-slot:cell(delete)="{ item }" v-if="endpointDel !== undefined">
        <b-button  @click="deleteData(item.id,item.name)" variant="danger">delete</b-button>
      </template>
      <template #table-busy>
        <div class="text-center text-danger my-2">
          <b-spinner class="align-middle"></b-spinner>
          <strong>Loading...</strong>
        </div>
      </template>
    </b-table>
  </b-container>
</template>

<script>
import ApiConnect from "@/services/ApiConnect";

export default {
  name: "dataTable",
  props:{
    endpointGet: String,
    endpointEdit: String,
    endpointDel: String,
    endpointCreate: String,
    type: String,
    fields: [],
    sortBy: String,
    parse: {type: Function},
    tableId: String,
  },
  data(){
    return{
      sortDesc: false,
      Count: 0,
      isBusy: false,
      perPageBook: 10,
      currentPageBook: 1,
      sortByVal: this.sortBy,
      oldData: undefined
    }
  },
  methods: {
    myProvider() {
      this.isBusy = true
      let promise = ApiConnect.get(this.endpointGet);
      return promise.then((data) =>{
        const items = data.data
        this.isBusy = false
        this.Count = items.length
        const parsedItems = this.customSort(this.parse(items,this.oldData))
        this.oldData = parsedItems
        return (parsedItems.slice((this.currentPageBook-1)*this.perPageBook,this.perPageBook*this.currentPageBook))
      }).catch(error => {
        this.isBusy = false
        console.log('err',error)
        return []
      })
    },
    deleteData(id,name){
      if (confirm("Are you sure you want to delete " + name)){
        ApiConnect.delete(this.endpointDel+id).then(response =>{
          console.log(response)
          this.makeToast(this.type+' '+name+' has been deleted successfully.','success');
          this.$root.$emit('bv::refresh::table', this.tableId)
        }).catch(err =>{
          console.log(err)
          this.makeToast('error:\n'+err,'danger');
        });
      }
    },
    makeToast(text,variant) {
      this.$bvToast.toast(text, {
        title: 'Library',
        variant: variant,
        autoHideDelay: 5000,
      })
    },
    customSort(data){
      const sortBy = this.sortByVal;
      const sortDesc = this.sortDesc;
      data.sort(customSort);
      function customSort(a, b) {
        var keyA = a[sortBy],
            keyB = b[sortBy];
        // Compare the 2 dates
        if(sortDesc){
          if (keyA < keyB) return -1;
          if (keyA > keyB) return 1;
        }else{
          if (keyA < keyB) return 1;
          if (keyA > keyB) return -1;
        }
        return 0;
      }
      return data;
    }
  },
  computed: {
    rows() {
      return this.Count;
    },
    endpointCreateComp() {
      if (this.endpointCreate === undefined){
        return this.endpointEdit+'0'
      }else{
        return this.endpointCreate
      }
    }
  }
}
</script>

<style scoped>

</style>