<script lang="ts">
	import { Checkbox } from "src/lib/components/ui/checkbox";
	import { Input } from "src/lib/components/ui/input";
	import { Textarea } from "src/lib/components/ui/textarea";
	import FileUpload from "./FileUpload.svelte";
	import ListInput from "./ListInput.svelte";
	import type { AssignmentFieldType } from "src/types";

	export let value: any;
	export let type: AssignmentFieldType;
	export let subType: AssignmentFieldType | undefined = undefined;
</script>

{#if type == 'ShortText'}
    <Input bind:value={value} />
{:else if type == 'LongText'}
    <Textarea class="h-[200px]" bind:value={value} />
{:else if type == 'Integer'}
    <Input type="number" bind:value={value} on:change={() => (value = Number(value))} />
{:else if type == 'Float'}
    <Input type="number" step="any" bind:value={value} on:change={() => (value = Number(value))} />
{:else if type == 'Boolean'}
    <Checkbox bind:checked={value} />
{:else if type == 'URL'}
    <Input bind:value={value} />
{:else if type == 'File'}
    <FileUpload bind:file={value}/>
{:else if type == 'List' && subType}
    <ListInput {subType} bind:value={value} />
{/if}