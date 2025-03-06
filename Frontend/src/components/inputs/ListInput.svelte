<script lang="ts">
	import type { AssignmentFieldType } from 'src/types/';
	import CustomButton from '../CustomButton.svelte';
	import { Plus } from 'lucide-svelte';
	import Trash_2 from 'lucide-svelte/icons/trash-2';
	import DeliveryFieldInput from './DeliveryFieldInput.svelte';

	export let value: any[];
	export let subType: AssignmentFieldType;

	if (value == undefined) {
		value = [];
	}

	function addItem() {
		value.push(undefined);
		value = value;
	}

	function removeItem(item: any) {
		const index = value.indexOf(item, 0);
		if (index > -1) {
			value.splice(index, 1);
			value = value;
		}
	}
</script>

{#each value as item}
	<div class="flex gap-2">
		<DeliveryFieldInput bind:value={item} type={subType} />
		<CustomButton color="red" outline={true} on:click={() => removeItem(item)}>
			<Trash_2 size="16" />
		</CustomButton>
	</div>
{/each}

<div class="flex items-center justify-center">
	<CustomButton color={'green'} outline={true} on:click={addItem}>
		<Plus size="16" />
	</CustomButton>
</div>
