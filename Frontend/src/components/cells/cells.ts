import StringCell from './StringCell.svelte';
import { createRender } from 'svelte-headless-table';
import RangeCell from './RangeCell.svelte';
import ListCell from './ListCell.svelte';
import FileCell from './FileCell.svelte';
import JsonCell from './JsonCell.svelte';
import type { AnalysisFieldType } from 'src/types';

export const getCell = (type: AnalysisFieldType) => {
    switch (type) {
        default:
            return ({ value }: { value: any }) => createRender(StringCell, { value });
    }
};