<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import Plus from 'lucide-svelte/icons/plus';
	import Button from '$lib/components/ui/button/button.svelte';

    export let addFunction: (input: string, file: File | null) => void;
    let studentInput: string;
    let file: File | null;
    let fileInput: HTMLInputElement;

    function handleFileUpload(event: Event): void
    {
        const input = event.target as HTMLInputElement;
        file = input.files ? input.files[0] : null;
    }

    function openFileExplorer() { fileInput.click(); }
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img width="48" height="48" src="https://img.icons8.com/fluency/480/csv.png" alt="csv" />
			<Card.Title class="ml-4 text-3xl">Add Students</Card.Title>
		</div>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col items-center">
			<p>
				Requires the student in the format: <br />
				<b>email1, email2, email3, ...</b><br />
				<br />
				Copy and Paste, Drag and Drop file, or
				<a on:click={openFileExplorer} href="#" class="text-blue-500 hover:text-blue-700">Browse Files</a>
                <input bind:this={fileInput} type="file" on:change={handleFileUpload} style="display: none;" />
			</p>
			<textarea
				class="mb-5 mt-5 h-64 w-2/3 border-2 border-dotted border-gray-500"
				placeholder="ola@nordmann.no, Ola Nordmann
kari@nordmann.no, Kari Nordmann"
                bind:value={studentInput}
			/><!-- Find better solution for placeholder with multiple lines? -->
			<Button
				class="mb-5 flex h-9 flex-row justify-between gap-2 bg-button-green pl-2 pr-3 hover:bg-button-green-hover"
                on:click={async () => addFunction(studentInput, file)}
			>
				<Plus />
				<p>Add</p>
			</Button>
		</div>
	</Card.Content>
</Card.Root>
