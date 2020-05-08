<template>
  <div>
    <q-table  
      title="Sizes"    
      :loading="!value"
      :data="rows"
      :columns="columns"
      row-key="row"
      :pagination.sync="pagination"
    >
      <template v-slot:top-right>
        <q-btn color="primary" label="Add size" @click="addSize" />
      </template>
      <template v-slot:body-cell-actions="props">
        <q-td :props="props">
          <q-btn icon="edit" dense flat round @click="editSize(props.row.index)" />
          <q-btn icon="delete" dense flat round @click="$refs.deleteDialog.show(props.row)" />
        </q-td>
      </template>
    </q-table>
    <q-dialog
      :value="editIndex !== null"
      @input="() => editIndex = null"
      persistent
      transition-show="scale"
      transition-hide="scale"
    >
      <q-card style="min-width: 300px">
        <q-form @submit="saveEdit" @reset="resetEdit">
          <q-card-section>
            <div class="text-h6">Edit size</div>
          </q-card-section>
          <q-card-section class="q-pt-none">
            <SizeForm v-model="editee" />
          </q-card-section>
          <q-card-actions align="right">
            <q-btn :disable="saving" label="Abort" type="reset" color="secondary" />
            <q-btn :loading="saving" label="Save" type="submit" color="primary" />
          </q-card-actions>
        </q-form>
      </q-card>
    </q-dialog>
    <DeleteDialog :nameField="row => row.tag" :deleteCallback="delSize" ref="deleteDialog" />
  </div>
</template>

<script>
import SizeForm from 'components/SizeForm';
import DeleteDialog from 'components/DeleteDialog';

export default {
  name: 'SizeConfigurationTable',
  components: { SizeForm, DeleteDialog },
  props: {
    value: {
      type: Array,
      default: () => []
    },
    saveCallback: {
      type: Function,
      default: () => () => Promise.reject("nothing saved")
    }
  },
  computed: {
    rows () {
      return this.$props.value ? this.$props.value.map((value, index) => ({ index, ...value })) : [];
    }
  },
  data () {
    return {
      columns: [
        { name: 'tag', label: 'Tag', align: 'left', field: row => row.tag, sortable: true },
        { name: 'maxWidth', label: 'Max width', align: 'right', field: row => row.maxWidth, sortable: true },
        { name: 'maxHeight', label: 'Max height', align: 'right', field: row => row.maxHeight, sortable: true },        
        { name: 'crop', label: 'Crop', align: 'left', field: row => row.crop, format: val => val ? `${val.aspectRatio}, ${val.cropStrategy}${val.color ? `, #${val.color}` : ''}` : '-' },
        { name: 'quality', label: 'Quality', align: 'right', field: row => row.quality, sortable: true },
        { name: 'format', label: 'Format', align: 'left', field: row => row.format, sortable: true },
        { name: 'actions', label: '', align: 'left' }
      ],
      editIndex: null,
      editee: null,
      saving: false,
      pagination: { rowsPerPage: 10 }
    }
  },
  methods: {
    addSize () {
      this.editee = { quality: 0.75, format: 'jpg' };
      this.editIndex = -1;
    },
    editSize (index) {      
      this.editee = JSON.parse(JSON.stringify(this.$props.value[index]));
      this.editIndex = index;
    },
    async delSize (row) {
      this.saving = true;
      try {
        let list = [...this.$props.value];
        list.splice(row.index, 1);
        await this.$props.saveCallback(list);
      } finally {
        this.saving = false;
      }
    },
    async saveEdit () {
      this.saving = true;
      try {
        let list = [...this.$props.value];
        if (this.editIndex === -1) {
          list.push(this.editee)
        } else {
          list[this.editIndex] = this.editee;
        }

        await this.$props.saveCallback(list);
        this.resetEdit();
      }
      finally {
        this.saving = false;
      }
    },
    resetEdit() {
      this.editIndex = null;
      this.editee = null;
    }
  }
}
</script>