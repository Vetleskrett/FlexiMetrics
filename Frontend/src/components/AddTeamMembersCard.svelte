<script lang="ts">
	import type { Team } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
    import { Progress } from "$lib/components/ui/progress/index.js"
    import Plus from 'lucide-svelte/icons/plus';
    import Button from '$lib/components/ui/button/button.svelte';

</script>

<Card.Root class="w-full overflow-hidden p-0">
    <Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="48"
				height="48"
				src="https://img.icons8.com/fluency/480/csv.png"
				alt="csv"
			/>
			<Card.Title class="ml-4 text-3xl">Add Team Members</Card.Title>
		</div>
    </Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col items-center">
            <p>
                Requires one line per team in the format: <br>
                <b>teamNumber, email, email, ...</b><br>
                <br>
                Copy and Paste, Drag and Drop file, or <a href="#" class="text-blue-500 hover:text-blue-700">Browse Files</a>
            </p>
            <textarea
                class="w-2/3 h-64 mt-5 mb-5 border-2 border-dotted border-gray-500"
                placeholder="1,abc@email.com,def@email.com,ghi@email.com 
2,jkl@email.com, mno@email.com"
            /><!-- Find better solution for placeholder with multiple lines? -->
            <Button
                class="flex h-9 flex-row justify-between gap-2 bg-button-green pl-2 pr-3 hover:bg-button-green-hover mb-5"
                >
                <Plus />
                <p>Add</p>
            </Button>
		</div>
	</Card.Content>
</Card.Root>