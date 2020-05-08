<template>
  <div>
    <q-table      
      title="Accounts"
      :data="rows"
      :columns="columns"
      row-key="row"
      :pagination.sync="pagination"
      :loading="!value"
    >
      <template v-slot:top-right>
        <q-btn color="primary" label="Add account" @click="addAccount" />
      </template>
      <template v-slot:body-cell-actions="props">
        <q-td :props="props">
          <q-btn icon="edit" dense flat round @click="editAccount(props.row.index)" />
          <q-btn icon="delete" dense flat round @click="$refs.deleteDialog.show(props.row)" />
        </q-td>
      </template>
      <template v-slot:body-cell-permissions="props">
        <q-td :props="props">
          <q-icon name="vpn_key" size="sm" color="primary" class="q-mr-sm"> 
            <q-tooltip self="top left" anchor="bottom left">
              <div v-for="perm in props.row.permissions" :key="perm">{{perm}}</div>
            </q-tooltip>
          </q-icon>
          <span>{{ props.row.permissions.length }}x</span>
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
            <div class="text-h6">Edit account</div>
          </q-card-section>
          <q-card-section class="q-pt-none">
            <AccountForm v-model="editee" />
          </q-card-section>
          <q-card-actions align="right">
            <q-btn :disable="saving" label="Abort" type="reset" color="secondary" />
            <q-btn :loading="saving" label="Save" type="submit" color="primary" />
          </q-card-actions>
        </q-form>
      </q-card>
    </q-dialog>
    <DeleteDialog :nameField="row => row.name" :deleteCallback="delAccount" ref="deleteDialog" />
  </div>
</template>

<script>
import AccountForm from 'components/AccountForm';
import DeleteDialog from 'components/DeleteDialog';

export default {
  name: 'AccountTable',
  components: { AccountForm, DeleteDialog },
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
        { name: 'name', label: 'Name', align: 'left', field: row => row.name, sortable: true },
        { name: 'type', label: 'Type', align: 'left', field: row => row.type, sortable: true },
        { name: 'permissions', label: 'Permissions', align: 'left', field: row => row.permissions },
        { name: 'actions', label: '', align: 'left' }
      ],
      editIndex: null,
      editee: null,
      saving: false,
      pagination: { rowsPerPage: 10 }
    }
  },
  methods: {
    addAccount () {
      this.editee = { 
        type: 'Basic', 
        name: 'New-user',
        permissions: ['thumbnail:read'],
        password: null,
      };
      this.editIndex = -1;
    },
    editAccount (index) {      
      this.editee = JSON.parse(JSON.stringify(this.$props.value[index]));
      this.editIndex = index;
    },
    async delAccount (row) {
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