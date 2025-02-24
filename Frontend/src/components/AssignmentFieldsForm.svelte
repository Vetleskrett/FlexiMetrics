<script lang="ts">
	import type { AssignmentFieldFormData } from 'src/types';
	import CustomButton from './CustomButton.svelte';
	import { Input } from '$lib/components/ui/input';
	import Trash_2 from 'lucide-svelte/icons/trash-2';
	import Plus from 'lucide-svelte/icons/plus';
	import * as Form from 'src/lib/components/ui/form';
	import type { SuperForm } from 'sveltekit-superforms';
	import * as Select from '$lib/components/ui/select/index';
	import Separator from 'src/lib/components/ui/separator/separator.svelte';

	export let form: SuperForm<any, any>;
	export let fields: AssignmentFieldFormData[];

	function addField() {
		fields.push({
			name: '',
			type: {
				label: 'Short Text',
				value: 'ShortText'
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

<div class="mt-4 flex flex-col gap-6">
	{#each fields as field, i}
		<Separator />
		<div class="flex flex-col gap-2 px-6 py-0">
			<Form.Field {form} name="fields[{i}].name" class="w-full">
				<Form.Control let:attrs>
					<Form.Label for="fields[{i}].name">Name</Form.Label>
					<Input {...attrs} id="fields[{i}].name" bind:value={field.name} />
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>

			<Form.Field {form} name="fields[{i}].type" class="w-[280px]">
				<Form.Control let:attrs>
					<Form.Label for="fields[{i}].type">Type</Form.Label>
					<Select.Root {...attrs} bind:selected={field.type}>
						<Select.Trigger>
							<Select.Value />
						</Select.Trigger>
						<Select.Content>
							<Select.Item value="ShortText" label="Short Text">Short Text</Select.Item>
							<Select.Item value="LongText" label="Long Text">Long Text</Select.Item>
							<Select.Item value="Integer" label="Integer">Integer</Select.Item>
							<Select.Item value="Float" label="Float">Float</Select.Item>
							<Select.Item value="Boolean" label="Boolean">Boolean</Select.Item>
							<Select.Item value="URL" label="URL">URL</Select.Item>
							<Select.Item value="JSON" label="JSON">JSON</Select.Item>
							<Select.Item value="File" label="File">File</Select.Item>
							<Select.Item value="List" label="List">List</Select.Item>
						</Select.Content>
					</Select.Root>
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>

			{#if field.type.value == 'Integer' || field.type.value == 'Float' || field.type.value == 'ShortText' || field.type.value == 'LongText'}
				<Form.Field {form} name="fields[{i}].min" class="w-[280px]">
					<Form.Control let:attrs>
						<Form.Label for="fields[{i}].min">
							{field.type.value == 'ShortText' || field.type.value == 'LongText'
								? 'Min Length'
								: 'Min'}
						</Form.Label>
						<Input {...attrs} type="number" id="fields[{i}].min" bind:value={field.min} />
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
				<Form.Field {form} name="fields[{i}].max" class="w-[280px]">
					<Form.Control let:attrs>
						<Form.Label for="fields[{i}].max">
							{field.type.value == 'ShortText' || field.type.value == 'LongText'
								? 'Max Length'
								: 'Max'}
						</Form.Label>
						<Input {...attrs} type="number" id="fields[{i}].max" bind:value={field.max} />
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
			{/if}
			{#if field.type.value == 'ShortText' || field.type.value == 'LongText' || field.type.value == 'URL'}
				<Form.Field {form} name="fields[{i}].regex">
					<Form.Control let:attrs>
						<Form.Label for="fields[{i}].regex">Regex</Form.Label>
						<Input {...attrs} id="fields[{i}].regex" bind:value={field.regex} />
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
			{/if}
			{#if field.type.value == 'List'}
				<Form.Field {form} name="fields[{i}].subType" class="w-[280px]">
					<Form.Control let:attrs>
						<Form.Label for="fields[{i}].subType">Sub Type</Form.Label>
						<Select.Root {...attrs} bind:selected={field.subType}>
							<Select.Trigger>
								<Select.Value />
							</Select.Trigger>
							<Select.Content>
								<Select.Item value="ShortText" label="Short Text">Short Text</Select.Item>
								<Select.Item value="LongText" label="Long Text">Long Text</Select.Item>
								<Select.Item value="Integer" label="Integer">Integer</Select.Item>
								<Select.Item value="Float" label="Float">Float</Select.Item>
								<Select.Item value="Boolean" label="Boolean">Boolean</Select.Item>
								<Select.Item value="URL" label="URL">URL</Select.Item>
								<Select.Item value="JSON" label="JSON">JSON</Select.Item>
							</Select.Content>
						</Select.Root>
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
			{/if}

			<div class="self-center">
				<CustomButton color="red" outline={true} on:click={() => removeField(field)}>
					<Trash_2 size="16" />
				</CustomButton>
			</div>
		</div>
	{/each}
	<Separator />
	<div class="flex items-center justify-center">
		<CustomButton color={'green'} outline={true} on:click={addField}>
			<Plus size="16" />
		</CustomButton>
	</div>
</div>
