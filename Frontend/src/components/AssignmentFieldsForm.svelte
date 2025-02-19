<script lang="ts">
	import type { AssignmentFieldType, AssignmentFieldFormData } from 'src/types';
	import CustomButton from './CustomButton.svelte';
	import { Input } from '$lib/components/ui/input';
	import Trash_2 from 'lucide-svelte/icons/trash-2';
	import Plus from 'lucide-svelte/icons/plus';
	import * as Form from 'src/lib/components/ui/form';
	import type { SuperForm } from 'sveltekit-superforms';
	import * as Select from '$lib/components/ui/select/index';

	export let form: SuperForm<any, any>;
	export let fields: AssignmentFieldFormData[];

	function addField() {
		fields.push({
			name: '',
			type: {
				label: 'String',
				value: 'String'
			}
		});
		fields = fields;
	}

	function removeField(field: AssignmentFieldFormData) {
		const index = fields.indexOf(field, 0);
		if (index > -1) {
			fields.splice(index, 1);
			fields = fields;
		}
	}
</script>

<div class="mt-4 flex flex-col gap-2">
	{#each fields as field, i}
		<div class="flex w-full gap-2">
			<Form.Field {form} name="fields[{i}].name" class="w-full">
				<Form.Control let:attrs>
					<Form.Label for="fields[{i}].name">Name</Form.Label>
					<Input
						{...attrs}
						id="fields[{i}].name"
						bind:value={field.name}
					/>
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>

			<Form.Field {form} name="fields[{i}].type" class="w-full">
				<Form.Control let:attrs>
					<Form.Label for="fields[{i}].type">Type</Form.Label>
					<Select.Root {...attrs} bind:selected={field.type}>
						<Select.Trigger>
							<Select.Value />
						</Select.Trigger>
						<Select.Content>
							<Select.Item value="String" label="String">String</Select.Item>
							<Select.Item value="Integer" label="Integer">Integer</Select.Item>
							<Select.Item value="Double" label="Double">Double</Select.Item>
							<Select.Item value="Boolean" label="Boolean">Boolean</Select.Item>
							<Select.Item value="Range" label="Range">Range</Select.Item>
							<Select.Item value="File" label="File">File</Select.Item>
						</Select.Content>
					</Select.Root>
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>

			{#if field.type.value == 'Range'}
				<Form.Field {form} name="fields[{i}].rangeMin" class="w-full">
					<Form.Control let:attrs>
						<Form.Label for="fields[{i}].rangeMin">Min</Form.Label>
						<Input
							{...attrs}
							type="number"
							id="fields[{i}].rangeMin"
							bind:value={field.rangeMin}
						/>
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
				<Form.Field {form} name="fields[{i}].rangeMax" class="w-full">
					<Form.Control let:attrs>
						<Form.Label for="fields[{i}].rangeMax">Max</Form.Label>
						<Input
							{...attrs}
							type="number"
							id="fields[{i}].rangeMax"
							bind:value={field.rangeMax}
						/>
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
			{/if}

			<div class="mt-8">
				<CustomButton color="red" outline={true} on:click={() => removeField(field)}>
					<Trash_2 size="16" on:click={() => removeField(field)} />
				</CustomButton>
			</div>
		</div>
	{/each}
	<div class="flex items-center justify-center">
		<CustomButton color={'green'} outline={true} on:click={addField}>
			<Plus size="16" />
		</CustomButton>
	</div>
</div>